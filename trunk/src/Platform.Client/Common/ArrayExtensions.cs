using System.Linq;
using System.Text;

namespace Platform.Client.Common
{
	public static class ArrayExtensions
	{
		public static string ToHexString(this byte[] bytes)
		{
			return bytes.Aggregate(new StringBuilder(bytes.Length * 2), (sb, i) => sb.Append(i.ToString("x2"))).ToString();
		}
	}
}
