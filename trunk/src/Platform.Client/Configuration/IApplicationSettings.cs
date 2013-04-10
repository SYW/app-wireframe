namespace Platform.Client.Configuration
{
	public interface IApplicationSettings
	{
		long AppId { get; }
		string AppSecret { get; }
		string CookieName { get; }
	}
}