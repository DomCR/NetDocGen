using NetDocGen.Services;
using NetDocGen.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetDocGen.Tests.Services
{
	public class MarkDownWikiGeneratorTests
	{
		[Fact]
		public void GenerateTest()
		{
			MarkDownWikiGenerator generator = new MarkDownWikiGenerator();

			AssemblyDocumentation doc = new AssemblyDocumentation(Assembly.LoadFrom(TestVariables.MockAssemblyPath));
			XmlParser parser = new XmlParser(TestVariables.MockAssemblyXmlPath);
			parser.ParseAssembly(doc);

			generator.Generate(doc, Path.Combine(TestVariables.OutputMarkdownFolder, "MockAssembly"));
		}
	}
}
