using Platform.Client;
using Platform.Client.Common;
using Platform.Client.Common.Context;
using Platform.Client.Common.WebClient;
using SywApplicationWireframe.Domain.Configuration;

namespace SywApplicationWireframe.Domain
{
	public abstract class ApiBase
	{
		protected IPlatformProxy Proxy { get; private set; }

		protected abstract string BasePath { get; }

		protected ApiBase(IContextProvider contextProvider)
		{
			var platformTokenProvider = new PlatformTokenProvider(contextProvider);
			Proxy = new PlatformProxy(new ApplicationSettings(), new WebClientBuilder(), new PlatformSettings(),
			                          platformTokenProvider,
			                          new PlatformHashProvider(new ApplicationSettings(), platformTokenProvider),
			                          new ParametersTranslator());
		}

		protected string GetEndpointPath(string endpoint)
		{
			return string.Format("/{0}/{1}", BasePath, endpoint);
		}

		protected T Get<T>(string endpoint, object parametersModel = null)
		{
			return Proxy.Get<T>(GetEndpointPath(endpoint), parametersModel);
		}

		protected T Post<T>(string endpoint, object parametersModel = null)
		{
			return Proxy.Post<T>(GetEndpointPath(endpoint), parametersModel);
		}
	}
}
