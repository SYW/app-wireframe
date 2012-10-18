namespace Platform.Client.Common.Context
{
	public interface IContextProvider
	{
		T Get<T>(string key);
		void Set<T>(string key, T value);
	}
}
