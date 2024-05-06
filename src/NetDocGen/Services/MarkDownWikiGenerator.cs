using NetDocGen.Markdown;
using NetDocGen.Pages;

namespace NetDocGen.Services
{
	public class MarkDownWikiGenerator : IMarkDownWikiGenerator
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
			this.creatFooter(documentation, outputFolder);
			this.createPages(documentation, outputFolder);
		}

		private void creatFooter(AssemblyDocumentation documentation, string outputFolder)
		{
			string path = Path.Combine(outputFolder, "_Footer.md");

			this._footerBuilder.AppendLine($"{documentation.Name} Class Library Documentation");

			string netdocgen = MarkdownFileBuilder.LinkString("NetDocGen", "https://github.com/DomCR/NetDocGen");
			this._footerBuilder.AppendLine($"Documentation created by {netdocgen}");

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
