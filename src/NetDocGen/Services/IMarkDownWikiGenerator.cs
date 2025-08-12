namespace NetDocGen.Services
{
	public interface IMarkdownWikiGenerator
	{
		void Generate(AssemblyDocumentation documentation, string outputFolder);
	}
}
