using System;

namespace Platform.Client.Common.Exceptions
{
	public class InvalidTokenException : Exception
	{
		public InvalidTokenException(Exception inner) :
			base("token has expired or is invalid", inner)
		{}
	}
}