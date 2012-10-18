using System.Text;

namespace Platform.Client.Common.WebClient
{
	public interface IWebClientBuilder
	{
		IWebClientBuilder SetTimeOut(int timeout);
		IWebClientBuilder SetReadWriteTimeOut(int timeout);
		IWebClientBuilder SetEncoding(Encoding encoding);		
		IWebClient Create();
	}


	public class WebClientBuilder : IWebClientBuilder
	{
		private int _timeout;
		private int _readwriteTimeout;
		private Encoding _encoding;		

		public WebClientBuilder()
		{
			// Defaults
			_encoding = Encoding.UTF8;
			_timeout = 30000; // 30 seconds default timeout
			_readwriteTimeout = 5000; // Allow 5 seconds between reads or writes by default
		}

		public IWebClientBuilder SetTimeOut(int timeout)
		{
			_timeout = timeout;
			return this;
		}

		public IWebClientBuilder SetReadWriteTimeOut(int timeout)
		{
			_readwriteTimeout = timeout;
			return this;
		}

		public IWebClientBuilder SetEncoding(Encoding encoding)
		{
			_encoding = encoding;
			return this;
		}

		public IWebClient Create()
		{
			return new WebClient
			{
				RequestTimeout = _timeout,		
				Encoding = _encoding,
				ReadWriteTimeout = _readwriteTimeout
			};
		}
	}
}
