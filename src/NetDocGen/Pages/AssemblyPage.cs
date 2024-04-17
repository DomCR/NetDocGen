using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDocGen.Pages
{
	public class AssemblyPage : DocumentationPage
	{
		private readonly AssemblyDocumentation _documentation;

		public AssemblyPage(AssemblyDocumentation documentation, string folder) : base(documentation.FullName, folder)
		{
			this._documentation = documentation;
		}

		protected override void build()
		{
			foreach (NamespaceDocumentation ns in this._documentation.Namespaces.OrderBy(n => n.FullName))
			{
				this.builder.Header(2, ns.FullName);

				foreach (TypeDocumentation t in ns.Types.OrderBy(t => t.Name))
				{
					this.builder.ListLink(t.Name, $"./{t.Name}.md");

					TypePage tpage = new TypePage(t, outputFolder);
					tpage.Create();
				}

				this.builder.AppendLine();
			}
		}
	}
}
