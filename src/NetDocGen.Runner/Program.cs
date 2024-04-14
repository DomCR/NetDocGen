using Microsoft.Extensions.DependencyInjection;
using NetDocGen.Markdown;
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

		static void Main(string[] args)
		{
			ServiceCollection services = new ServiceCollection();
			services.RegisterDependencies();
			services.BuildServiceProvider();

			AssemblyDocumentation doc;

#if true
			doc = new AssemblyDocumentation(Assembly.LoadFrom(acadPath));
#else
			using (XmlParser parser = new XmlParser(acadXmlPath))
			{
				doc = parser.ParseAssembly();
			}
#endif
			MarkDownGenerator generator = new MarkDownGenerator(outputFolder);
			generator.Generate(doc);

			Console.WriteLine("PROGRAM END");
		}
	}
}
