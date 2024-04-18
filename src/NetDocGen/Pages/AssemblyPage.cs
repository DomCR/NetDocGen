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

		public AssemblyPage(AssemblyDocumentation documentation, string folder) : base(documentation.Name, folder)
		{
			this._documentation = documentation;
		}

		protected override void build()
		{
			foreach (NamespaceDocumentation ns in this._documentation.Namespaces.OrderBy(n => n.FullName))
			{
				NamespacePage nsPage = new NamespacePage(ns, OutputFolder);
				nsPage.Create();

				this.builder.Header(2, ns.FullName, Path.Combine(ns.FullName, ns.FullName));

				foreach (TypeDocumentation t in ns.Types.OrderBy(t => t.Name))
				{
					this.builder.ListLink(t.Name, Path.Combine(ns.FullName, t.Name));
				}

				this.builder.AppendLine();
			}
		}
	}

	public class NamespacePage : DocumentationPage
	{
		private readonly NamespaceDocumentation _documentation;

		public NamespacePage(NamespaceDocumentation documentation, string outputFolder) : base(documentation.FullName, outputFolder)
		{
			this._documentation = documentation;
			this.OutputFolder = Path.Combine(outputFolder, documentation.FullName);
		}

		protected override void build()
		{
			if (!Directory.Exists(OutputFolder))
			{
				Directory.CreateDirectory(this.OutputFolder);
			}

			foreach (TypeDocumentation t in _documentation.Types.OrderBy(t => t.Name))
			{
				this.builder.ListLink(t.Name, $"./{t.Name}");

				TypePage tpage = new TypePage(t, OutputFolder);
				tpage.Create();
			}
		}
	}
}
