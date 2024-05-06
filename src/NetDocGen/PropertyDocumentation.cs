using System.Reflection;

namespace NetDocGen
{
	public class PropertyDocumentation : MemberDocumentation<PropertyInfo, TypeDocumentation>
	{
		public string ValueDescription { get; set; }

		public PropertyDocumentation(string fullName) : base(fullName) { }

		public PropertyDocumentation(PropertyInfo property) : base(property)
		{
		}

		public PropertyDocumentation(PropertyInfo property, TypeDocumentation owner) : base(property, owner)
		{
		}
	}
}
