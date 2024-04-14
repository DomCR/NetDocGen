using System.Reflection;

namespace NetDocGen
{
	public class TypeDocumentation : MemberDocumentation<Type, CommonDocumentation>
	{
		public override string FullName { get; }

		public List<MethodDocumentation> Methods { get; } = new();

		public List<PropertyDocumentation> Properties { get; } = new();

		public List<TypeDocumentation> NestedTypes { get; } = new();

		public TypeDocumentation(string fullName) : base(fullName)
		{
			this.FullName = fullName;
		}

		public TypeDocumentation(Type type) : this(type, null)
		{
		}

		public TypeDocumentation(Type type, NamespaceDocumentation owner) : base(type, owner)
		{
			this.FullName = type.FullName;
			this.processMembers();
		}

		private void processMembers()
		{
			foreach (MethodInfo m in _info.GetMethods(BindingFlags.Public
											| BindingFlags.Instance
											| BindingFlags.DeclaredOnly))
			{
				if (m.IsSpecialName)
					continue;

				MethodDocumentation mdoc = new MethodDocumentation(m, this);
				this.Methods.Add(mdoc);
			}

			foreach (PropertyInfo p in _info.GetProperties(BindingFlags.Public
														| BindingFlags.Instance
														| BindingFlags.DeclaredOnly))
			{
				PropertyDocumentation pdoc = new PropertyDocumentation(p, this);
				this.Properties.Add(pdoc);
			}

			foreach (var item in _info.GetNestedTypes(BindingFlags.Public
														| BindingFlags.Instance
														| BindingFlags.DeclaredOnly))
			{

			}
		}
	}

	public class PropertyDocumentation : MemberDocumentation<PropertyInfo, TypeDocumentation>
	{
		public PropertyDocumentation(string fullName) : base(fullName) { }

		public PropertyDocumentation(PropertyInfo property) : base(property)
		{
		}

		public PropertyDocumentation(PropertyInfo property, TypeDocumentation owner) : base(property, owner)
		{
		}
	}

	public class MethodDocumentation : MemberDocumentation<MethodInfo, TypeDocumentation>
	{
		public override string FullName { get; }

		public MethodDocumentation(string fullName) : base(fullName)
		{
			this.FullName = fullName;
		}

		public MethodDocumentation(MethodInfo method) : base(method)
		{
		}

		public MethodDocumentation(MethodInfo property, TypeDocumentation owner) : base(property, owner)
		{
		}
	}
}
