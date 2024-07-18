using System.ComponentModel;
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
		public const string VisibilityPublic = "public";
		public const string VisibilityProtectedInternal = "protected internal";
		public const string VisibilityProtected = "protected";
		public const string VisibilityInternal = "internal";
		public const string VisibilityPrivate = "private";

		public static string GetSignature(this MemberInfo member)
		{
			StringBuilder str = new StringBuilder();

			foreach (string att in getRellevantAttributes(member))
			{
				str.AppendLine(att);
			}

			str.Append(getVisibility(member));
			str.Append(" ");

			str.AppendJoin(" ", getQualifiers(member));

			str.AppendJoin(" ", getKind(member));

			str.AppendJoin(" ", getReturningType(member));
			str.Append(" ");

			str.AppendJoin(" ", member.GetMemberName());

			if (member is PropertyInfo propertyInfo)
			{
				str.Append(" ");
				str.AppendJoin(" ", getPropertyGetSet(propertyInfo));
			}

			return str.ToString();
		}

		public static Type GetReturningType<T>(this T member)
			where T : MemberInfo
		{
			switch (member)
			{
				case EventInfo eventInfo:
					throw new NotImplementedException();
				case FieldInfo field:
					return field.FieldType;
				case MethodInfo methodBase:
					return methodBase.ReturnType;
				case PropertyInfo property:
					return property.PropertyType;
				default:
					throw new NotSupportedException($"{member.GetType().FullName} not supported");
			}
		}

		public static string GetMemberName<T>(this T member)
			where T : MemberInfo
		{
			switch (member)
			{
				case EventInfo eventInfo:
					throw new NotImplementedException();
				case FieldInfo field:
					return field.Name;
				case MethodBase methodBase:
					return methodBase.GetMethodName();
				case PropertyInfo property:
					return property.Name;
				case Type type:
					return type.GetTypeName();
				default:
					throw new NotSupportedException($"{member.GetType().FullName} not supported");
			}
		}

		public static string GetMemberFullName<T>(this T member)
			where T : MemberInfo
		{
			switch (member)
			{
				case EventInfo eventInfo:
					throw new NotImplementedException();
				case FieldInfo field:
					throw new NotImplementedException();
				case MethodBase methodBase:
					return methodBase.GetMethodFullName();
				case PropertyInfo property:
					return property.GetPropertyFullName();
				case Type type:
					return $"{type.Namespace}.{type.GetTypeName()}";
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
			return (parameterStrings.Count > 0) ? $"({string.Join(", ", parameterStrings)})" : string.Empty;
		}

		public static string GetTypeName(this Type type)
		{
			string name = string.Empty;

			if (type.IsByRef)
			{
				return $"ref {type.GetElementType().GetTypeName()}";
			}

			if (type.IsGenericType)
			{
				if (type.Name.Contains('`'))
				{
					var clean = type.Name.cleanGenericTypeName();

					var gen = type.GetGenericArguments();

					name = clean + "<" +
						string.Join(", ", type.GetGenericArguments()
						   .Select(argType => argType.GetTypeName())) + ">";
				}
				else
				{
					name = type.Name;
				}
			}
			else if (type.IsArray)
			{
				name = type.GetElementType().GetTypeName() +
					   "[" +
					   (type.GetArrayRank() > 1 ? new string(',', type.GetArrayRank() - 1) : string.Empty) +
					   "]";
			}
			else
			{
				name = type.Name;
			}

			return name;
		}

		private static string cleanGenericTypeName(this string genericTypeName)
		{
			var index = genericTypeName.IndexOf('`');
			return (index < 0) ? genericTypeName : genericTypeName.Substring(0, index);
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

			return fullTypeName.cleanGenericTypeName();
		}

		private static bool isIndexerProperty(MethodBase methodInfo)
		{
			return methodInfo.IsSpecialName && (methodInfo.Name == "get_Item" || methodInfo.Name == "set_Item");
		}

		private static string explicitImplicitPostfix(MethodBase methodInfo)
		{
			if (!methodInfo.IsSpecialName ||
				(methodInfo.Name != "op_Explicit" && methodInfo.Name != "op_Implicit"))
			{
				return string.Empty;
			}

			return "~" + getTypeId((methodInfo as MethodInfo).ReturnType);
		}

		private static string[] getGenericClassParams(MemberInfo info)
		{
			return info.DeclaringType.IsGenericType
				? info.DeclaringType.GetGenericArguments().Select(t => t.Name).ToArray()
				: Array.Empty<string>();
		}

		private static IEnumerable<string> getRellevantAttributes(MemberInfo member)
		{
			foreach (Attribute att in member.GetCustomAttributes())
			{
				yield return att.GetType().GetTypeName();
			}

			var obsoleteAttribute = member.GetCustomAttribute<ObsoleteAttribute>();
			if (obsoleteAttribute != null)
			{
				var message = obsoleteAttribute.Message;
				if (string.IsNullOrWhiteSpace(message))
				{
					yield return "[Obsolete]";
				}
				else
				{
					yield return $"[Obsolete({message})]";
				}
			}

			var browsableAttribute = member.GetCustomAttribute<EditorBrowsableAttribute>();
			if (browsableAttribute != null && browsableAttribute.State != EditorBrowsableState.Always)
			{
				yield return $"[EditorBrowsable({browsableAttribute.State})]";
			}

			var attributeUsage = member.GetCustomAttribute<AttributeUsageAttribute>();
			if (attributeUsage != null)
			{
				string attusage = $"[AttributeUsage({attributeUsage.ValidOn})";

				if (!attributeUsage.Inherited)
				{
					attusage += ", Inherited = false";
				}

				if (attributeUsage.AllowMultiple)
				{
					attusage += ", AllowMultiple = true";
				}

				attusage += ")]";
				yield return attusage;
			}

			if (member is TypeInfo typeInfo && typeInfo.IsEnum && typeInfo.GetCustomAttributes<FlagsAttribute>().Any())
			{
				yield return "[Flags]";
			}
		}

		private static string getVisibility(MemberInfo member)
		{
			switch (member)
			{
				case EventInfo eventInfo:
					return getMethodVisibility(eventInfo.AddMethod);
				case FieldInfo fieldInfo:
					return getFieldVisibility(fieldInfo);
				case MethodBase methodBase:
					return getMethodVisibility(methodBase);
				case PropertyInfo propertyInfo:
					return getPropertyVisibility(propertyInfo);
				case TypeInfo type:
					return getTypeVisibility(type);
				default:
					return string.Empty;
			}
		}

		private static string getTypeVisibility(TypeInfo typeInfo)
		{
			if (typeInfo.IsPublic || typeInfo.IsNestedPublic)
			{
				return VisibilityPublic;
			}

			if (typeInfo.IsNestedFamORAssem)
			{
				return VisibilityProtectedInternal;
			}

			if (typeInfo.IsNestedFamily)
			{
				return VisibilityProtected;
			}

			if (typeInfo.IsNestedAssembly || typeInfo.IsNestedFamANDAssem)
			{
				return VisibilityInternal;
			}

			return VisibilityPrivate;
		}

		private static string getMethodVisibility(MethodBase methodBase)
		{
			if (methodBase.IsPublic)
			{
				return VisibilityPublic;
			}

			if (methodBase.IsFamilyOrAssembly)
			{
				return VisibilityProtectedInternal;
			}

			if (methodBase.IsFamily)
			{
				return VisibilityProtected;
			}

			if (methodBase.IsAssembly || methodBase.IsFamilyAndAssembly)
			{
				return VisibilityInternal;
			}

			return VisibilityPrivate;
		}

		private static string getPropertyVisibility(PropertyInfo propertyInfo)
		{
			MethodInfo getMethod = propertyInfo.GetMethod;
			if (getMethod == null)
			{
				throw new InvalidOperationException();
			}

			return getMethodVisibility(propertyInfo.GetMethod);
		}

		private static string getFieldVisibility(FieldInfo fieldInfo)
		{
			if (fieldInfo.IsPublic)
			{
				return VisibilityPublic;
			}

			if (fieldInfo.IsFamilyOrAssembly)
			{
				return VisibilityProtectedInternal;
			}

			if (fieldInfo.IsFamily)
			{
				return VisibilityProtected;
			}

			if (fieldInfo.IsAssembly || fieldInfo.IsFamilyAndAssembly)
			{
				return VisibilityInternal;
			}

			return VisibilityPrivate;
		}

		private static IEnumerable<string> getQualifiers(MemberInfo memberInfo)
		{
			if (isStatic(memberInfo))
				yield return "static";
			else if (memberInfo is Type t && t.IsSealed)
				yield return "sealed";
			else if (isAbstract(memberInfo))
				yield return "abstract";
			else if (isVirtual(memberInfo))
				yield return "virtual";
			else if (isOverride(memberInfo))
				yield return "override";

			if (isConst(memberInfo))
				yield return "const";
			if (isReadOnly(memberInfo))
				yield return "readonly";
		}

		public static string getKind(MemberInfo memberInfo)
		{
			if (memberInfo is EventInfo)
			{
				return "event";
			}

			if (memberInfo is Type t)
			{
				TypeInfo info = t.GetTypeInfo();

				if (typeof(Delegate).GetTypeInfo().IsAssignableFrom(info))
					return "delegate";
				if (isRecord(info))
					return "record";
				if (info.IsClass)
					return "class";
				if (info.IsInterface)
					return "interface";
				if (info.IsEnum)
					return "enum";
				if (info.IsValueType)
					return "struct";
			}

			return string.Empty;
		}

		private static string getReturningType(MemberInfo member)
		{
			switch (member)
			{
				case EventInfo eventInfo:
					return eventInfo.EventHandlerType.GetMemberName();
				case FieldInfo fieldInfo:
					return fieldInfo.FieldType.GetMemberName();
				case MethodInfo methodBase:
					return methodBase.ReturnType.GetMemberName();
				case PropertyInfo propertyInfo:
					return propertyInfo.PropertyType.GetMemberName();
				default:
					return string.Empty;
			}
		}

		private static bool isRecord(Type type) => type.GetMethod("<Clone>$") != null;

		private static bool isStatic(MemberInfo memberInfo)
		{
			var typeInfo = memberInfo as TypeInfo;
			if (typeInfo != null)
				return typeInfo.IsClass && typeInfo.IsAbstract && typeInfo.IsSealed;

			var eventInfo = memberInfo as EventInfo;
			if (eventInfo != null)
				return eventInfo.AddMethod.IsStatic;

			var propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null)
				return (propertyInfo.GetMethod ?? propertyInfo.SetMethod)?.IsStatic ?? false;

			var fieldInfo = memberInfo as FieldInfo;
			if (fieldInfo != null)
				return fieldInfo.IsStatic && !fieldInfo.IsLiteral;

			var methodBase = memberInfo as MethodBase;
			if (methodBase != null)
				return methodBase.IsStatic;

			return false;
		}

		private static bool isAbstract(MemberInfo memberInfo)
		{
			var typeInfo = memberInfo as TypeInfo;
			if (typeInfo != null && !typeInfo.IsInterface)
				return typeInfo.IsAbstract;

			if (memberInfo.DeclaringType?.GetTypeInfo().IsInterface == true)
				return false;

			var eventInfo = memberInfo as EventInfo;
			if (eventInfo != null)
				return eventInfo.AddMethod != null && isAbstract(eventInfo.AddMethod);

			var propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null)
			{
				return (propertyInfo.GetMethod != null && isAbstract(propertyInfo.GetMethod)) ||
					(propertyInfo.SetMethod != null && isAbstract(propertyInfo.SetMethod));
			}

			var methodBase = memberInfo as MethodBase;
			if (methodBase != null)
				return methodBase.IsAbstract;

			return false;
		}

		private static bool isVirtual(MemberInfo memberInfo)
		{
			if (memberInfo.DeclaringType?.GetTypeInfo().IsInterface == true)
				return false;

			var eventInfo = memberInfo as EventInfo;
			if (eventInfo != null)
				return eventInfo.AddMethod != null && isVirtual(eventInfo.AddMethod);

			var propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null)
			{
				return (propertyInfo.GetMethod != null && isVirtual(propertyInfo.GetMethod)) ||
					(propertyInfo.SetMethod != null && isVirtual(propertyInfo.SetMethod));
			}

			var methodInfo = memberInfo as MethodInfo;
			if (methodInfo != null)
				return methodInfo.IsVirtual && !methodInfo.IsFinal && methodInfo.GetRuntimeBaseDefinition().DeclaringType == methodInfo.DeclaringType;

			return false;
		}

		private static bool isOverride(MemberInfo memberInfo)
		{
			if (memberInfo.DeclaringType?.GetTypeInfo().IsInterface == true)
				return false;

			var eventInfo = memberInfo as EventInfo;
			if (eventInfo != null)
				return eventInfo.AddMethod != null && isOverride(eventInfo.AddMethod);

			var propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null)
			{
				return (propertyInfo.GetMethod != null && isOverride(propertyInfo.GetMethod)) ||
					(propertyInfo.SetMethod != null && isOverride(propertyInfo.SetMethod));
			}

			var methodInfo = memberInfo as MethodInfo;
			if (methodInfo != null)
				return methodInfo.IsVirtual && !methodInfo.IsFinal && methodInfo.GetRuntimeBaseDefinition().DeclaringType != methodInfo.DeclaringType;

			return false;
		}

		private static bool isConst(MemberInfo memberInfo)
		{
			return (memberInfo as FieldInfo)?.IsLiteral ?? false;
		}

		private static bool isReadOnly(MemberInfo memberInfo)
		{
			return (memberInfo as FieldInfo)?.IsInitOnly ?? false;
		}

		private static string getPropertyGetSet(PropertyInfo propertyInfo)
		{
			var getMethod = propertyInfo.GetMethod;
			var setMethod = propertyInfo.SetMethod;
			if (getMethod == null && setMethod == null)
				throw new InvalidOperationException();

			if (getMethod != null && setMethod == null)
			{
				return "{ get; }";
			}

			if (getMethod == null)
			{
				return "{ set; }";
			}

			var getVisibility = getMethodVisibility(getMethod);
			var setVisibility = getMethodVisibility(setMethod);

			List<string> pars = new List<string>();
			pars.Add("{");
			if (getVisibility != VisibilityPublic)
			{
				pars.Add(getVisibility);
			}
			pars.Add("get;");

			if (setVisibility != VisibilityPublic)
			{
				pars.Add(setVisibility);
			}
			pars.Add("set;");
			pars.Add("}");


			StringBuilder str = new StringBuilder();
			str.AppendJoin(' ', pars);

			return str.ToString(); ;
		}

	}
}

