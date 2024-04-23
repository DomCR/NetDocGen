using NetDocGen.Markdown;
using NetDocGen.Utils;
using System.Text;

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

		public void CreateFile()
		{
			var sb = new StringBuilder();

			this.CreateIn(sb);

			File.WriteAllText(this.createFilePath(), sb.ToString());
		}

		public void CreateIn(StringBuilder output)
		{
			this.builder.Clear();

			this.builder.Header(1, this.title);

			this.build();

			output.Append(this.builder.ToString());
		}

		protected virtual void build() { }

		protected virtual string createFilePath()
		{
			string file = this.fileName;

			if (Path.GetExtension(file) != "md")
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