using System.Collections.Generic;

namespace NetDocGen
{
	public class NamespaceDocumentation : CommonDocumentation
	{
		public override string Name { get; }

		public override string FullName { get; }

		public AssemblyDocumentation Assembly { get; }

		public List<TypeDocumentation> Types { get; } = new List<TypeDocumentation>();

		public NamespaceDocumentation(string name, AssemblyDocumentation assembly)
		{
			this.Name = name;
			this.FullName = name;
			this.Assembly = assembly;
		}

		public override AssemblyDocumentation GetRoot()
		{
			return this.Assembly;
		}
	}
}