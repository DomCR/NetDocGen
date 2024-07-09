using CSUtilities;
using System.Runtime.CompilerServices;

namespace NetDocGen.Tests
{
	public static class TestVariables
	{
		public static string MockAssemblyPath { get { return EnvironmentVars.Get<string>("MOCK_ASSEMBLY_PATH"); } }

		public static string MockAssemblyXmlPath { get { return EnvironmentVars.Get<string>("MOCK_ASSEMBLY_XML_PATH"); } }

		public static string SamplesFolder { get { return EnvironmentVars.Get<string>("SAMPLES_FOLDER"); } }

		public static string OutputFolder { get { return EnvironmentVars.Get<string>("OUTPUT_FOLDER"); } }

		public static string OutputMarkdownFolder { get { return EnvironmentVars.Get<string>("OUTPUT_MARKDOWN_FOLDER"); } }

		public static bool LocalEnv { get { return EnvironmentVars.Get<bool>("LOCAL_ENV"); } }

		static TestVariables()
		{
			EnvironmentVars.SetIfNull("SAMPLES_FOLDER", "..\\..\\..\\..\\Test\\_samples");
			EnvironmentVars.SetIfNull("OUTPUT_FOLDER", "..\\..\\..\\..\\Test\\_output");
			EnvironmentVars.SetIfNull("LOCAL_ENV", "true");

			EnvironmentVars.SetIfNull("MOCK_ASSEMBLY_PATH", Path.Combine(TestVariables.SamplesFolder, "MockAssembly.dll"));
			EnvironmentVars.SetIfNull("MOCK_ASSEMBLY_XML_PATH", Path.Combine(TestVariables.SamplesFolder, "MockAssembly.xml"));
			EnvironmentVars.SetIfNull("OUTPUT_MARKDOWN_FOLDER", Path.Combine(OutputFolder, "markdown"));
		}

		public static void CreateOutputFolders()
		{
			if (!Directory.Exists(OutputFolder))
			{
				Directory.CreateDirectory(OutputFolder);
			}

			if (!Directory.Exists(OutputMarkdownFolder))
			{
				Directory.CreateDirectory(OutputMarkdownFolder);
			}
		}

		private static T getValue<T>([CallerMemberName] string str = null)
		{
			string name = string.Concat(
				str.Select((x, i) => i > 0
				&& char.IsUpper(x) ? "_"
				+ x.ToString() : x.ToString())).ToUpper();

			return EnvironmentVars.Get<T>(name);
		}
	}
}