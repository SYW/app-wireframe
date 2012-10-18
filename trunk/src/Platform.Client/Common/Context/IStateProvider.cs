using System;
using System.Collections.Generic;
using System.Web;

namespace Platform.Client.Common.Context
{
	public interface IStateProvider
	{
		string Get(string name);
		void Set(string name, string value);
	}

	public class CookieStateProvider : IStateProvider
	{
		public string Get(string name)
		{
			var val = HttpContext.Current.Request.Cookies.Get(name);
			return val == null ? null : val.Value;
		}

		public void Set(string name, string value)
		{
			var cookie = new HttpCookie(name, value) {Expires = DateTime.Now.AddHours(1)};

			HttpContext.Current.Response.Cookies.Set(cookie);
		}
	}

	public class ThreadStateProvider : IStateProvider
	{
		private static readonly object Latch = new object();
		[ThreadStatic]
		private static Dictionary<string, string> _store;
		private static Dictionary<string, string> Store
		{
			get
			{
				if (_store == null)
				{
					lock (Latch)
					{
						if (_store == null)
						{
							_store = new Dictionary<string, string>();
						}
					}
				}

				return _store;
			}
		}

		public string Get(string name)
		{
			return Store.ContainsKey(name) ? Store[name] : null;
		}

		public void Set(string name, string value)
		{
			Store[name] = value;
		}
	}
}
