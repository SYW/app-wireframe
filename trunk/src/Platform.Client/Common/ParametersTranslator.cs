using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Platform.Client.Common
{
	public interface IParametersTranslator
	{
		string ToJson(KeyValuePair<string, object> parameter);
	}

	public class ParametersTranslator : IParametersTranslator
	{
		private readonly Type[] _untypedCollectionInterfaces = new[] { typeof(IEnumerable), typeof(IList), typeof(ICollection) };
		private readonly Type[] _genericCollectionInterfaces = new[] { typeof(IEnumerable<>), typeof(IList<>), typeof(ICollection<>) };

		private bool IsCollectionInstance(Type type)
		{
			return type != typeof(string) && 
				type.GetInterfaces()
					.Any(typeToCheck => _untypedCollectionInterfaces.Any(untypedInteface => typeToCheck == untypedInteface) || 
						_genericCollectionInterfaces.Any(typedInterface => typedInterface == typeToCheck));
		}

		public string ToJson(KeyValuePair<string, object> parameter)
		{
			var parameterType = parameter.Value.GetType();

			if (parameterType == typeof(DateTime))
			{
				return ((DateTime)parameter.Value).ToString("yyyy-MM-ddTHH:mm:ss");
			}

			if (parameterType.IsValueType || parameterType == typeof(string))
			{
				return parameter.Value.ToString();
			}

			if (parameterType.IsArray || IsCollectionInstance(parameterType))
			{
				var objectArray = ((IEnumerable)parameter.Value).Cast<object>().Select(x => x.ToString());
				var stringifiedArray = String.Join(",", objectArray.ToArray());

				return stringifiedArray;
			}

			return JsonConvert.SerializeObject(parameter.Value);
		}
	}
}
