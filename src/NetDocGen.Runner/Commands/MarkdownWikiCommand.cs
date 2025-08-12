using McMaster.Extensions.CommandLineUtils;
using NetDocGen.Services;
using NetDocGen.Xml;
using System.Reflection;

namespace NetDocGen.Runner.Commands
{
	[Command(Name = "netdocgen", Description = "")]
	public class MarkdownWikiCommand : CommandBase
	{
		private readonly IMarkdownWikiGenerator _generator;

		public MarkdownWikiCommand(IMarkdownWikiGenerator generator)
		{
			this._generator = generator;
		}

		public void OnExecute()
		{
			this.validateOptions();

			this.processOptions();

			AssemblyDocumentation doc = new AssemblyDocumentation(Assembly.LoadFrom(Input));
			XmlParser parser = new XmlParser(this.XmlInput);
			parser.ParseAssembly(doc);

			this._generator.Generate(doc, this.Output);
		}
	}
}
