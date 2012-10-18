using Platform.Client.Common.Context;

namespace Platform.Client
{
	public interface IPlatformTokenProvider
	{
		void Set(string token);
		string Get();
	}

	public class PlatformTokenProvider : IPlatformTokenProvider
	{
		private const string TokenKey = "token";
		private readonly IContextProvider _contextProvider;

		public PlatformTokenProvider(IContextProvider contextProvider)
		{
			_contextProvider = contextProvider;
		}

		public void Set(string token)
		{
			_contextProvider.Set(TokenKey, token);
		}

		public string Get()
		{
			return _contextProvider.Get<string>(TokenKey);
		}
	}
}
