using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NetDocGen.Extensions
{
	/// <summary>
	/// Reflection extension methods.
	/// </summary>
	public static class ReflectionExtensions
	{
		public const string ConstructorNameID = "ctor";

		public static string GetTypeDefinition(this Type type)
		{
			string protection = string.Empty;
			if (type.IsPublic)
			{
				protection = "public";
			}

			string t = string.Empty;
			if (type.IsClass)
			{
				t = "class";
			}

			return $"{protection} {t} {type.Name}";
		}

		public static string GetMemberName<T>(this T member)
			where T : MemberInfo
		{
			switch (member)
			{
				case Type type:
					return type.Name;
				case PropertyInfo property:
					return property.Name;
				case MethodBase methodBase:
					return methodBase.GetMethodName();
				case EventInfo eventInfo:
					throw new NotImplementedException();
				default:
					throw new NotSupportedException($"{member.GetType().FullName} not supported");
			}
		}

		public static string GetMemberFullName<T>(this T member)
			where T : MemberInfo
		{
			switch (member)
			{
				case Type type:
					return type.FullName;
				case PropertyInfo property:
					return property.GetPropertyFullName();
				case MethodBase methodBase:
					return methodBase.GetMethodFullName();
				case EventInfo eventInfo:
					throw new NotImplementedException();
				default:
					throw new NotSupportedException($"{member.GetType().FullName} not supported");
			}
		}

		public static string GetPropertyFullName(this PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException(nameof(propertyInfo));
			}

			if ((propertyInfo.MemberType & MemberTypes.Property) == 0)
			{
				throw new ArgumentException(nameof(propertyInfo));
			}

			var getParameters = propertyInfo?.GetMethod?.GetParameters();
			if (getParameters?.Length > 0)
			{
				return getTypeId(propertyInfo.DeclaringType) + "." +
					   propertyInfo.Name +
					   getParametersXmlId(getParameters, getGenericClassParams(propertyInfo));
			}

			var setParameters = propertyInfo?.SetMethod?.GetParameters();
			if (setParameters?.Length > 1)
			{
				return getTypeId(propertyInfo.DeclaringType) + "." +
					   propertyInfo.Name +
					   getParametersXmlId(setParameters.Take(setParameters.Length - 1), getGenericClassParams(propertyInfo));
			}

			return getTypeId(propertyInfo.DeclaringType) + "." + propertyInfo.Name;
		}

		/// <summary>
		/// Get the short method name.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static string GetMethodName(this MethodBase method)
		{
			return $"{shortMethodName(method)}" +
				   getParametersXmlId(method.GetParameters(), getGenericClassParams(method)) +
				   explicitImplicitPostfix(method);
		}

		public static string GetMethodFullName(this MethodBase method)
		{
			if (method == null)
			{
				throw new ArgumentNullException(nameof(method));
			}

			return $"{getTypeId(method.DeclaringType)}.{method.GetMethodName()}";
		}

		private static string shortMethodName(MethodBase method)
		{
			if (method.IsConstructor)
			{
				return ConstructorNameID;
			}

			string generics = string.Empty;
			if (method.IsGenericMethod)
			{
				Type[] args = method.GetGenericArguments();
				generics = $"<{string.Join(",", args.Select(a => a.Name))}>";
			}

			return $"{(isIndexerProperty(method) ? "Item" : method.Name)}{generics}";
		}

		private static string getParametersXmlId(IEnumerable<ParameterInfo> parameters, string[] genericClassParams)
		{
			// Calculate the parameter string as this is in the member name in the XML
			var parameterStrings = parameters
				.Select(parameterInfo => getTypeId(
					parameterInfo.ParameterType,
					parameterInfo.IsOut || parameterInfo.ParameterType.IsByRef,
					isMethodParameter: true,
					genericClassParams))
				.ToList();
			return (parameterStrings.Count > 0) ? $"({string.Join(",", parameterStrings)})" : string.Empty;
		}

		private static string getTypeId(Type type, bool isOut = false, bool isMethodParameter = false, string[] genericClassParams = null)
		{
			if (type.IsGenericParameter)
			{
				return type.Name;
			}

			Type[] args = type.GetGenericArguments();
			string fullTypeName;
			var typeNamespace = type.Namespace == null ? string.Empty : $"{type.Namespace}.";
			var outString = isOut ? "@" : string.Empty;

			if (type.MemberType == MemberTypes.TypeInfo &&
				!type.IsGenericTypeDefinition &&
				(type.IsGenericType || args.Length > 0) && (!type.IsClass || isMethodParameter))
			{
				var paramString = string.Join(",",
					args.Select(o => getTypeId(o, isOut: false, isMethodParameter, genericClassParams)));
				var typeName = Regex.Replace(type.Name, "`[0-9]+", "{" + paramString + "}");
				fullTypeName = $"{typeNamespace}{typeName}{outString}";
			}
			else if (type.IsNested)
			{
				fullTypeName = $"{typeNamespace}{type.DeclaringType.Name}.{type.Name}{outString}";
			}
			else if (type.ContainsGenericParameters && (type.IsArray || type.GetElementType() != null))
			{
				var typeName = getTypeId(type.GetElementType(), isOut: false, isMethodParameter, genericClassParams);
				fullTypeName = $"{typeName}{(type.IsArray ? "[]" : string.Empty)}{outString}";
			}
			else
			{
				fullTypeName = $"{typeNamespace}{type.Name}{outString}";
			}

			fullTypeName = fullTypeName.Replace("&", string.Empty);

			// Multi-dimensional arrays must have 0: for each dimension. Eg. [,,] has to become [0:,0:,0:]
			while (fullTypeName.Contains("[,"))
			{
				var index = fullTypeName.IndexOf("[,");
				var lastIndex = fullTypeName.IndexOf(']', index);
				fullTypeName = fullTypeName.Substring(0, index + 1) +
					string.Join(",", Enumerable.Repeat("0:", lastIndex - index)) +
					fullTypeName.Substring(lastIndex);
			}
			return fullTypeName;
		}

		private static bool isIndexerProperty(MethodBase methodInfo)
		{
			return methodInfo.IsSpecialName && (methodInfo.Name == "get_Item" || methodInfo.Name == "set_Item");
		}

		private static string explicitImplicitPostfix(MethodBase methodInfo)
		{
			if (!methodInfo.IsSpecialName ||
				(methodInfo.Name != "op_Explicit" && methodInfo.Name != "op_Implicit")) return string.Empty;
			return "~" + getTypeId((methodInfo as MethodInfo).ReturnType);
		}

		private static string[] getGenericClassParams(MemberInfo info)
		{
			return info.DeclaringType.IsGenericType
				? info.DeclaringType.GetGenericArguments().Select(t => t.Name).ToArray()
				: Array.Empty<string>();
		}

		private static string genericParamPrefix(Type type, string[] genericClassParams)
		{
			return (genericClassParams != null && genericClassParams.Contains(type.Name)) ? "`" : "``";
		}
	}
}

