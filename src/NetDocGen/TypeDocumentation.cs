using System.Reflection;

namespace NetDocGen
{
	public class TypeDocumentation : MemberDocumentation<Type, CommonDocumentation>
	{
		public override string FullName { get; }

		public List<MethodDocumentation> Methods { get; } = new();

		public IEnumerable<PropertyDocumentation> Properties { get { return this._properties.Values; } }

		public List<TypeDocumentation> NestedTypes { get; } = new();

		public List<EventDocumentation> Events { get; } = new();

		private Dictionary<string, PropertyDocumentation> _properties = new();

		private Dictionary<string, MethodDocumentation> _methods = new();

		public TypeDocumentation(string fullName) : base(fullName)
		{
			this.FullName = fullName;
		}

		public TypeDocumentation(Type type) : this(type, null)
		{
		}

		public TypeDocumentation(Type type, CommonDocumentation owner) : base(type, owner)
		{
			this.FullName = this.removeInvalidCharacters(type.FullName);
			this.processMembers();
		}

		public void AddProperty(PropertyDocumentation property)
		{
			this._properties.Add(property.Name, property);
		}

		private void processMembers()
		{
			foreach (MethodInfo m in ReflectionInfo.GetMethods(BindingFlags.Public
											| BindingFlags.Instance
											| BindingFlags.DeclaredOnly))
			{
				if (m.IsSpecialName)
					continue;

				MethodDocumentation mdoc = new MethodDocumentation(m, this);
				this.Methods.Add(mdoc);
			}

			foreach (PropertyInfo p in ReflectionInfo.GetProperties(BindingFlags.Public
														| BindingFlags.Instance
														| BindingFlags.DeclaredOnly))
			{
				PropertyDocumentation pdoc = new PropertyDocumentation(p, this);
				this._properties.Add(pdoc.Name, pdoc);
			}

			foreach (FieldInfo f in ReflectionInfo.GetFields(BindingFlags.Public
											| BindingFlags.Instance
											| BindingFlags.DeclaredOnly))
			{
				//TODO: Add fieds in types
			}

			foreach (Type t in ReflectionInfo.GetNestedTypes(BindingFlags.Public
														| BindingFlags.Instance
														| BindingFlags.DeclaredOnly))
			{
				TypeDocumentation tdoc = new TypeDocumentation(t, this);
				this.NestedTypes.Add(tdoc);
			}

			foreach (EventInfo e in ReflectionInfo.GetEvents(BindingFlags.Public
												| BindingFlags.Instance
												| BindingFlags.DeclaredOnly))
			{
				EventDocumentation tdoc = new EventDocumentation(e, this);
				this.Events.Add(tdoc);
			}
		}

		internal PropertyDocumentation GetProperty(string name)
		{
			return this._properties[name];
		}
	}
}
