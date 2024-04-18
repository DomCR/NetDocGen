using NetDocGen.Markdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDocGen.Pages
{
	public abstract class DocumentationPage
	{
		public string OutputFolder { get; protected set; }

		protected readonly MarkdownFileBuilder builder;

		protected readonly string title;

		protected DocumentationPage(string title, string outputFolder)
		{
			this.title = title;
			this.OutputFolder = outputFolder;

			builder = new MarkdownFileBuilder();
		}

		public void Create()
		{
			this.builder.Header(1, this.title);

			this.build();

			File.WriteAllText(this.createFilePath(), this.builder.ToString());
		}

		protected virtual void build() { }

		protected virtual string createFilePath()
		{
			string filename = $"{this.title}.md";
			filename = string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

			return Path.Combine(OutputFolder, filename);
		}
	}
}