namespace NetDocGen.Services
{
	public interface IMarkDownGenerator
	{
		void Generate(AssemblyDocumentation documentation, string outputFolder);
	}
}
