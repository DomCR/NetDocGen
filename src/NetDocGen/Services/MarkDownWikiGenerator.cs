using NetDocGen.Markdown;
using NetDocGen.Pages;

namespace NetDocGen.Services
{
	public class MarkDownWikiGenerator : IMarkDownGenerator
	{
		private readonly MarkdownFileBuilder _sidebarBuilder = new();

		private readonly MarkdownFileBuilder _footerBuilder = new();

		public void Generate(AssemblyDocumentation documentation, string outputFolder)
		{
			if (!Directory.Exists(outputFolder))
			{
				Directory.CreateDirectory(outputFolder);
			}

			this.createSidebar(documentation, outputFolder);
			this.creatFooter(outputFolder);
			this.createPages(documentation, outputFolder);
		}

		private void creatFooter(string outputFolder)
		{
			string path = Path.Combine(outputFolder, "_Footer.md");
			File.WriteAllText(path, this._footerBuilder.ToString());
		}

		private void createPages(AssemblyDocumentation documentation, string outputFolder)
		{
			AssemblyPage assemblyPage = new AssemblyPage(documentation, outputFolder);
			assemblyPage.CreateFile();
		}

		private void createSidebar(AssemblyDocumentation documentation, string outputFolder)
		{
			foreach (NamespaceDocumentation ns in documentation.Namespaces.OrderBy(n => n.FullName))
			{
				this._sidebarBuilder.ListLink(ns.FullName, ns.FullName);
			}

			string path = Path.Combine(outputFolder, "_Sidebar.md");
			File.WriteAllText(path, this._sidebarBuilder.ToString());
		}
	}
}
