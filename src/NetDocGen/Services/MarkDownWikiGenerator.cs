using NetDocGen.Markdown;
using NetDocGen.Pages;

namespace NetDocGen.Services
{
	public class MarkDownWikiGenerator : IMarkDownGenerator
	{
		public string OutputFolder { get; }

		private readonly AssemblyDocumentation _documentation;

		private readonly MarkdownFileBuilder _sidebarBuilder = new();

		private readonly MarkdownFileBuilder _footerBuilder = new();

		public MarkDownWikiGenerator(AssemblyDocumentation documentation, string outputFolder)
		{
			this.OutputFolder = outputFolder;
			this._documentation = documentation;
		}

		public void Generate()
		{
			this.createSidebar();
			this.creatFooter();
			this.createPages();
		}

		private void creatFooter()
		{
			string path = Path.Combine(OutputFolder, "_Footer.md");
			File.WriteAllText(path, this._footerBuilder.ToString());
		}

		private void createPages()
		{
			AssemblyPage assemblyPage = new AssemblyPage(_documentation, OutputFolder);
			assemblyPage.Create();
		}

		private void createSidebar()
		{
			foreach (NamespaceDocumentation ns in this._documentation.Namespaces.OrderBy(n => n.FullName))
			{
				this._sidebarBuilder.Link(ns.FullName, ns.FullName);
				this._sidebarBuilder.AppendLine();
			}

			string path = Path.Combine(OutputFolder, "_Sidebar.md");
			File.WriteAllText(path, this._sidebarBuilder.ToString());
		}
	}
}
