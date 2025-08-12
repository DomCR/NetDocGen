using NetDocGen.Services;
using NetDocGen.Xml;
using System.Reflection;

namespace NetDocGen.Tests.Services
{
	public class MarkdownWikiGeneratorTests
	{
		[Fact]
		public void GenerateTest()
		{
			MarkdownWikiGenerator generator = new MarkdownWikiGenerator();

			AssemblyDocumentation doc = new AssemblyDocumentation(Assembly.LoadFrom(TestVariables.MockAssemblyPath));
			XmlParser parser = new XmlParser(TestVariables.MockAssemblyXmlPath);
			parser.ParseAssembly(doc);

			generator.Generate(doc, Path.Combine(TestVariables.OutputMarkdownFolder, "MockAssembly"));
		}
	}
}
