using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Platform.Client.Common
{
	public class SignatureBuilder
	{
		private readonly List<byte> _input = new List<byte>();

		public SignatureBuilder Append(IList<byte> bytes)
		{
			_input.AddRange(bytes);
			return this;
		}

		public string Create()
		{
			var hashAlgorithm = SHA256.Create();
			return hashAlgorithm.ComputeHash(_input.ToArray()).ToHexString().ToLower();
		}
	}

	public static class SignatureBuilderExtensions
	{
		public static SignatureBuilder Append(this SignatureBuilder builder, long val)
		{
			var bytes = BitConverter.GetBytes(val);
			return builder.Append(bytes);
		}

		public static SignatureBuilder Append(this SignatureBuilder builder, DateTime val)
		{
			var bytes = BitConverter.GetBytes(val.ToUnixTime());
			return builder.Append(bytes);
		}

		public static SignatureBuilder Append(this SignatureBuilder builder, string val)
		{
			var bytes = Encoding.UTF8.GetBytes(val);
			return builder.Append(bytes);
		}
	}
}
