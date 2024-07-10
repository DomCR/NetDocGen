using NetDocGen.Extensions;
using System.Reflection;

namespace NetDocGen
{
	public class MethodDocumentation : MemberDocumentation<MethodInfo, TypeDocumentation>
	{
		public MethodDocumentation(string fullName) : base(fullName)
		{
			this._fullName = fullName;
		}

		public MethodDocumentation(MethodInfo method) : base(method)
		{
		}

		public MethodDocumentation(MethodInfo method, TypeDocumentation owner) : base(method, owner)
		{
		}
	}
}
