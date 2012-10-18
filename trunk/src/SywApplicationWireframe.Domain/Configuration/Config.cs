using System;
using System.Configuration;

namespace SywApplicationWireframe.Domain.Configuration
{
	public static class Config
	{
		public static long GetLong(string key)
		{
			var val = GetString(key);

			long result;
			if (!long.TryParse(val, out result))
				throw new ConfigurationErrorsException(String.Format("Value of key '{0}' was expected to be long but wasn't: {1}", key, val));

			return result;
		}

		public static string GetString(string key)
		{
			var val = ConfigurationManager.AppSettings[key];

			if (string.IsNullOrEmpty(val))
				throw new ConfigurationErrorsException(
					String.Format("Key '{0}' is missing from appSettings in the configuration. Add it to your config file", key));

			return val;
		}

		public static Uri GetUri(string key)
		{
			var val = GetString(key);

			Uri result;

			if (!Uri.TryCreate(val, UriKind.Absolute, out result))
				throw new ConfigurationErrorsException(
					String.Format("Value of key '{0}' is not a valid URI: '{1}'", key, val));

			return result;
		}
	}
}
