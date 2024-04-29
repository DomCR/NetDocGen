namespace NetDocGen.Services
{
	public interface IMarkDownWikiGenerator
	{
		void Generate(AssemblyDocumentation documentation, string outputFolder);
	}
}
