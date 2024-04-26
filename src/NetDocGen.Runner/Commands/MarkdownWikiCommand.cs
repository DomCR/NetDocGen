using CSUtilities.Extensions;
using McMaster.Extensions.CommandLineUtils;
using NetDocGen.Services;
using NetDocGen.Xml;
using System.Reflection;

namespace NetDocGen.Runner.Commands
{
	[Command(Name = "netdocgen", Description = "")]
	public class MarkdownWikiCommand : CommandBase
	{
		private readonly IMarkDownGenerator _generator;

		public MarkdownWikiCommand(IMarkDownGenerator generator)
		{
			this._generator = generator;
		}

		public void OnExecute()
		{
			this.validateOptions();

			this.processOptions();

			AssemblyDocumentation doc = new AssemblyDocumentation(Assembly.LoadFrom(Input));
			using (XmlParser parser = new XmlParser(this.XmlInput))
			{
				parser.ParseAssembly(doc);
			}

			this._generator.Generate(doc, this.Output);
		}
	}

	public abstract class CommandBase
	{
		[Argument(0, Description = "Input assembly file path")]
		public string Input { get; set; } = string.Empty;

		[Option(ShortName = "x", LongName = "xml", Description = "Input xml documentaiton file path")]
		public string XmlInput { get; set; } = string.Empty;

		[Option(ShortName = "o", LongName = "output", Description = "Output directory path")]
		public string Output { get; set; }

		[Option(ShortName = "c", LongName = "clear", Description = "Clear output directory")]
		public bool ClearOutputDirectory { get; set; } = false;

		protected void validateOptions()
		{
			Input.TrowIfNullOrEmpty();
			if (!File.Exists(Input))
			{
				throw new ArgumentException();
			}

			Output.TrowIfNullOrEmpty();

			if (XmlInput.IsNullOrEmpty())
			{
				XmlInput = Path.ChangeExtension(this.Input, "xml");
			}
			if (!File.Exists(XmlInput))
			{
				throw new ArgumentException();
			}
		}

		protected void processOptions()
		{
			if(ClearOutputDirectory && Directory.Exists(Output))
			{
				Directory.Delete(Output, true);
			}
		}
	}
}
