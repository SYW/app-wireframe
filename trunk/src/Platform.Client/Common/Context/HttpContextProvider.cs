using System.Web;

namespace Platform.Client.Common.Context
{
	public class HttpContextProvider : IContextProvider
	{
		public T Get<T>(string key)
		{
			return (T)HttpContext.Current.Items[key];
		}

		public void Set<T>(string key, T value)
		{
			HttpContext.Current.Items[key] = value;
		}
	}
}
