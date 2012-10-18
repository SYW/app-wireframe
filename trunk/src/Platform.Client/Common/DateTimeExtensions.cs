using System;

namespace Platform.Client.Common
{
	public static class DateTimeExtensions
	{
		public static double ToUnixTime(this DateTime target)
		{
			return (target - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
		}
	}
}
