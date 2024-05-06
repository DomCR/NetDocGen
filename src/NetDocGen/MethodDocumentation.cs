using System.Reflection;

namespace NetDocGen
{
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
