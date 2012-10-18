using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;

namespace Platform.Client.Common.WebClient
{
	public interface IWebClient
	{
		string UploadValues(Uri uri, string method, NameValueCollection data);
		string UploadValues(Uri uri, string method, string data);
		string DownloadString(string url);
		string DownloadString(Uri uri);
		T GetJson<T>(Uri url);
		T GetJson<T>(Uri url, NameValueCollection data);
	}

	public class WebClient : IWebClient
	{
		public WebClient()
		{			
			Encoding = Encoding.UTF8;
			RequestTimeout = 30000; // 30 seconds default timeout
			ReadWriteTimeout = 5000; // Allow 5 seconds between reads or writes by default.
		}

		public int RequestTimeout { get; set; }

		public int ReadWriteTimeout { get; set; }

		public Encoding Encoding { get; set; }

		public Action<WebRequest> CustomizeRequest;

		public string UploadValues(Uri uri, string method, NameValueCollection data)
		{
			return UploadValues(uri, method, UrlEncode(data));
		}

		public string UploadValues(Uri uri, string method, string data)
		{
			method = method.ToUpper();
			switch (method)
			{
				case "POST":
				case "PUT":
					return PostRequest(uri, method, data);

				case "GET":
					return GetRequest(uri, method, data);
				default:
					throw new NotSupportedException("Supported methods include GET/PUT/POST while the method has been invoked with: " + method);
			}
		}

		private string GetRequest(Uri uri, string method, string data)
		{
			uri = new Uri(uri + "?" + data);
			var request = GetWebRequest(uri);
			request.Method = method;
			return ReadResponse(request);
		}

		private string PostRequest(Uri uri, string method, string data)
		{
			var request = GetWebRequest(uri);
			request.Method = method;

			if (method == "POST")
				request.ContentType = "application/x-www-form-urlencoded";

			var bytes = Encoding.GetBytes(data);
			request.ContentLength = bytes.Length;
			using (var dataStream = request.GetRequestStream())
				dataStream.Write(bytes, 0, bytes.Length);
			return ReadResponse(request);
		}

		public string DownloadString(string url)
		{
			return DownloadString(new Uri(url));
		}

		public string DownloadString(Uri uri)
		{
			var request = GetWebRequest(uri);
			request.Method = "GET";
			return ReadResponse(request);
		}

		public T GetJson<T>(Uri url)
		{
			var response = DownloadString(url);
			return JsonConvert.DeserializeObject<T>(response);
		}

		public T GetJson<T>(Uri url, NameValueCollection data)
		{
			var response = UploadValues(url, "GET", data);
			return JsonConvert.DeserializeObject<T>(response);
		}

		private string UrlEncode(NameValueCollection data)
		{
			return data.AllKeys
				.Aggregate(new StringBuilder(), (sb, k) => sb
															   .Append(HttpUtility.UrlEncode(k, Encoding))
															   .Append("=")
															   .Append(HttpUtility.UrlEncode(data[k], Encoding))
															   .Append("&")
				)
				.ToString();
		}

		private string ReadResponse(WebRequest request)
		{
			using (var response = request.GetResponse())
			using (var reader = new StreamReader(response.GetResponseStream(), Encoding))
			{
				return reader.ReadToEnd();
			}	
		}

		private WebRequest GetWebRequest(Uri url)
		{
			var request = WebRequest.Create(url);
			CustomizeRequestInternal(request);
			return request;
		}

		private void CustomizeRequestInternal(WebRequest wr)
		{
			wr.Timeout = RequestTimeout;

			var hwr = wr as HttpWebRequest;
			if (hwr != null)
			{
				hwr.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				hwr.ReadWriteTimeout = ReadWriteTimeout;
			}
		}
	}
}
