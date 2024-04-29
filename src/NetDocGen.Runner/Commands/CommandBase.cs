using CSUtilities;
using CSUtilities.Extensions;
using McMaster.Extensions.CommandLineUtils;
using System.Runtime.CompilerServices;

namespace NetDocGen.Runner.Commands
{
	public abstract class CommandBase
	{
		[Argument(0, Description = "Input assembly file path")]
		public string Input
		{
			get { return this.getValue<string>(); }
			set { this.setValue(value); }
		}

		[Option(ShortName = "x", LongName = "xml", Description = "Input xml documentaiton file path")]
		public string XmlInput
		{
			get { return this.getValue<string>(); }
			set { this.setValue(value); }
		}

		[Option(ShortName = "o", LongName = "output", Description = "Output directory path")]
		public string Output
		{
			get { return this.getValue<string>(); }
			set { this.setValue(value); }
		}

		[Option(ShortName = "c", LongName = "clear", Description = "Clear output directory")]
		public bool ClearOutputDirectory
		{
			get { return this.getValue<bool>(); }
			set { this.setValue(value); }
		}

		protected void validateOptions()
		{
			Input.TrowIfNullOrEmpty("Input assembly path cannot be null or empty");
			if (!File.Exists(Input))
			{
				throw new ArgumentException($"Input file {Input} doesn't exists");
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
			if (ClearOutputDirectory && Directory.Exists(Output))
			{
				Directory.Delete(Output, true);
			}
		}

		private T getValue<T>([CallerMemberName] string str = null)
		{
			string name = string.Concat(
				str.Select((x, i) => i > 0
				&& char.IsUpper(x) ? "_"
				+ x.ToString() : x.ToString())).ToUpper();

			return EnvironmentVars.Get<T>(name);
		}

		private void setValue<T>(T value, [CallerMemberName] string str = null)
		{
			string name = string.Concat(
				str.Select((x, i) => i > 0
				&& char.IsUpper(x) ? "_"
				+ x.ToString() : x.ToString())).ToUpper();

			EnvironmentVars.Set(name, value.ToString());
		}
	}
}
