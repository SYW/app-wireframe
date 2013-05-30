using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Platform.Client.Common;
using Platform.Client.Common.Exceptions;
using Platform.Client.Common.WebClient;
using Platform.Client.Configuration;

namespace Platform.Client
{
	public interface IPlatformProxy
	{
		T AnonymousGet<T>(string servicePath, object parametersModel = null);
		T SecuredAnonymousGet<T>(string servicePath, object parametersModel = null);
		T Get<T>(string servicePath, object parametersModel = null);
	}

	public class PlatformProxy : IPlatformProxy
	{
		public static ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> ParametersModelCache = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

		private readonly IWebClientBuilder _webClientBuilder;
		private readonly IPlatformSettings _platformSettings;
		private readonly IPlatformTokenProvider _platformTokenProvider;
		private readonly IPlatformHashProvider _platformHashProvider;
		private readonly IParametersTranslator _parametersTranslator;

		public PlatformProxy(IPlatformSettings platformSettings,
			IApplicationSettings applicationSettings,
			IPlatformTokenProvider platformTokenProvider)
		{
			_platformSettings = platformSettings;
			_platformTokenProvider = platformTokenProvider;

			_parametersTranslator = new ParametersTranslator();
			_webClientBuilder = new WebClientBuilder();
			_platformHashProvider = new PlatformHashProvider(applicationSettings, platformTokenProvider);
		}

		public T AnonymousGet<T>(string servicePath, object parametersModel = null)
		{
			return Get<T>(GetServiceUrl(servicePath), GetParameters(parametersModel), null);
		}

		public T SecuredAnonymousGet<T>(string servicePath, object parametersModel = null)
		{
			var serviceUrl = GetSecureServiceUrl(servicePath);

			return Get<T>(serviceUrl, GetParameters(parametersModel), null);
		}

		public T Get<T>(string servicePath, object parametersModel = null)
		{
			return Get<T>(GetServiceUrl(servicePath), GetParameters(parametersModel), AddContextParameters);
		}

		private ICollection<KeyValuePair<string, object>> GetParameters(object parametersModel)
		{
			if (parametersModel == null)
				return new KeyValuePair<string, object>[0];

			var type = parametersModel.GetType();

			if (!ParametersModelCache.ContainsKey(type))
				ParametersModelCache.TryAdd(type, type.GetProperties(BindingFlags.Instance | BindingFlags.Public));

			var modelPropertiesInfo = ParametersModelCache[type];

			return modelPropertiesInfo
				.Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(parametersModel, null)))
				.ToArray();
		}

		private T Get<T>(Uri serviceUrl, ICollection<KeyValuePair<string, object>> parameters, Action<NameValueCollection> applyExtraParameters)
		{
			var webClient = _webClientBuilder.Create();

			NameValueCollection serviceParameters;

			if (parameters == null)
			{
				serviceParameters = new NameValueCollection(2);
			}
			else
			{
				serviceParameters = new NameValueCollection(parameters.Count + 2);

				foreach (var parameter in parameters)
				{
					var value = _parametersTranslator.ToJson(parameter);

					serviceParameters.Add(parameter.Key, value);
				}

			}

			if (applyExtraParameters != null)
				applyExtraParameters(serviceParameters);

			try
			{
				return webClient.GetJson<T>(serviceUrl, serviceParameters);
			}
			catch (WebException ex)
			{
				throw GeneratePlatformRequestException(ex);
			}
		}

		private Exception GeneratePlatformRequestException(WebException ex)
		{
			try
			{
				var readError = ReadError(ex);
				
				var errorDto = JsonConvert.DeserializeObject<RequestExceptionDto>(readError).Error;
				if (errorDto.StatusCode == 401)
					return new InvalidTokenException(ex);
				
				return new RequestException(errorDto.StatusCode, errorDto.Message, errorDto.RequestId, ex);
			}
			catch (Exception)
			{
				return ex;
			}
		}

		private string ReadError(WebException ex)
		{
			using (var reader = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8))
			{
				return reader.ReadToEnd();
			}
		}

		private void AddContextParameters(NameValueCollection serviceParameters)
		{
			serviceParameters.Add("token", _platformTokenProvider.Get());
			serviceParameters.Add("hash", _platformHashProvider.GetHash());
		}

		private Uri GetServiceUrl(string servicePath)
		{
			return new Uri(_platformSettings.ApiUrl, servicePath);
		}

		private Uri GetSecureServiceUrl(string servicePath)
		{
			return new Uri(_platformSettings.SecureApiUrl, servicePath);
		}
	}
}
