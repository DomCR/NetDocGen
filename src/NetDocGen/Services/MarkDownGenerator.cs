using NetDocGen.Pages;

namespace NetDocGen.Services
{
	public class MarkDownGenerator : IMarkDownGenerator
	{
		public string OutputFolder { get; }

		public MarkDownGenerator(string outputFolder)
		{
			this.OutputFolder = outputFolder;
		}

		public void Generate(AssemblyDocumentation documentation)
		{
			AssemblyPage assemblyPage = new AssemblyPage(documentation, OutputFolder);

			assemblyPage.Create();
		}
	}
}
