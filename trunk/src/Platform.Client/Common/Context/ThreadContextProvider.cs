using System;
using System.Collections.Generic;

namespace Platform.Client.Common.Context
{
	public class ThreadContextProvider : IContextProvider
	{
		private static readonly object Latch = new object();
		[ThreadStatic]
		private static Dictionary<string, object> _store;
		private static Dictionary<string, object> Store
		{
			get
			{
				if (_store == null)
				{
					lock(Latch)
					{
						if (_store == null)
						{
							_store = new Dictionary<string, object>();
						}
					}
				}

				return _store;
			}
		}

		public T Get<T>(string key)
		{
			return Store.ContainsKey(key) ? (T)Store[key] : default(T);
		}

		public void Set<T>(string key, T value)
		{
			Store[key] = value;
		}
	}
}
