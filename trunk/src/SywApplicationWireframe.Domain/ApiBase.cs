using System.Collections.Generic;
using System.Linq;
using Platform.Client;
using Platform.Client.Common.Context;
using SywApplicationWireframe.Domain.Configuration;
using SywApplicationWireframe.Domain.Users;

namespace SywApplicationWireframe.Domain
{
	public abstract class ApiBase
	{
		protected IPlatformProxy Proxy { get; private set; }

		protected abstract string BasePath { get; }

		protected ApiBase(IContextProvider contextProvider)
		{
			Proxy = new PlatformProxy(new PlatformSettings(),
									   new ApplicationSettings(),
									   new PlatformTokenProvider(contextProvider));
		}

		protected string GetEndpointPath(string endpoint)
		{
			return string.Format("/{0}/{1}", BasePath, endpoint);
		}

		protected T Call<T>(string endpoint, object parametersModel = null)
		{
			return Proxy.Get<T>(GetEndpointPath(endpoint), parametersModel);
		}
	}
}
