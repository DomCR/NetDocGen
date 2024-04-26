using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using NetDocGen.Runner.Commands;
using NetDocGen.Services;
using NetDocGen.Xml;
using System.Reflection;

namespace NetDocGen.Runner
{
	internal class Program
	{
		const string outputFolder = "..\\..\\..\\..\\..\\local\\output\\ACadSharp";
		const string pathDll = "..\\..\\..\\..\\Test\\MockAssembly\\bin\\Debug\\net6.0\\MockAssembly.dll";
		const string pathDoc = "..\\..\\..\\..\\Test\\MockAssembly\\bin\\Debug\\net6.0\\MockAssembly.xml";
		const string acadPath = "..\\..\\..\\..\\..\\..\\ACadSharp\\ACadSharp\\bin\\Debug\\net6.0\\ACadSharp.dll";
		const string acadXmlPath = "..\\..\\..\\..\\..\\..\\ACadSharp\\ACadSharp\\bin\\Debug\\net6.0\\ACadSharp.xml";

		static int Main(string[] args)
		{
			try
			{
				var services = new ServiceCollection()
				.RegisterDependencies()
				.BuildServiceProvider();

				using (var cli = new CommandLineApplication<MarkdownWikiCommand>())
				{
					cli.Conventions.UseDefaultConventions()
						.UseConstructorInjection(services);

					return cli.Execute(args);
				}
			}
			catch (Exception ex)
			{

				throw;
			}


			AssemblyDocumentation doc;

			using (XmlParser parser = new XmlParser(acadXmlPath))
			{
				doc = parser.ParseAssembly(Assembly.LoadFrom(acadPath));
			}

			System.IO.DirectoryInfo di = new DirectoryInfo(outputFolder);
			foreach (FileInfo file in di.GetFiles())
			{
				file.Delete();
			}
			foreach (DirectoryInfo dir in di.GetDirectories())
			{
				dir.Delete(true);
			}

			Console.WriteLine("PROGRAM END");
		}
	}

	public class Configuration
	{
		public string Output { get; set; }
	}
}
