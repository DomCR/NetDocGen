using NetDocGen.Markdown;
using NetDocGen.Utils;

namespace NetDocGen.Pages
{
	public abstract class DocumentationPage
	{
		public string OutputFolder { get; protected set; }

		protected abstract string title { get; }

		protected abstract string fileName { get; }

		protected readonly MarkdownFileBuilder builder;

		protected DocumentationPage(string outputFolder)
		{
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
			string file = this.fileName;

			if (!Path.HasExtension(file))
			{
				file = $"{fileName}.md";
			}

			return Path.Combine(OutputFolder, file);
		}

		protected void buildDataTable(int level, string title, IEnumerable<CommonDocumentation> data, bool linkName = false)
		{
			if (!data.Any())
			{
				return;
			}

			builder.Header(level, title);

			string[] cols = new string[] { "Name", "Summary" };

			List<List<string>> rows = new();
			foreach (CommonDocumentation item in data)
			{
				string name = linkName ?
					MarkdownFileBuilder.LinkString(item.Name, PathUtils.ToLink(item.FullName)) : item.Name;

				rows.Add(new List<string>()
				{
					name,
					item.Summary
				});
			}

			builder.Table(
				cols,
				alignment: new string[] { ":-", ":-" },
				items: rows);
		}
	}
}