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

		public static DocumentationPage GeneratePage<T>(string outputFolder, T documentation)
			where T : CommonDocumentation
		{
			switch (documentation)
			{
				case AssemblyDocumentation assemblyDocumentation:
					return new AssemblyPage(outputFolder, assemblyDocumentation);
			case MethodDocumentation methodDocumentation:
					return new MethodPage(outputFolder, methodDocumentation);
				case PropertyDocumentation propertyDocumentation:
					return new PropertyPage(outputFolder, propertyDocumentation);
				case TypeDocumentation typeDocumentation:
					return new TypePage(outputFolder, typeDocumentation);
				default:
					throw new NotSupportedException($"[{typeof(T).FullName}] not supported");
			}
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

		protected void buildDataTable<T>(int level, string title, IEnumerable<T> data, bool createSubpages = false)
			where T : CommonDocumentation
		{
			if (!data.Any())
			{
				return;
			}

			builder.Header(level, title);

			string[] cols = new string[] { "Name", "Summary" };

			List<List<string>> rows = new();
			foreach (T item in data)
			{
				string name = string.Empty;
				if (createSubpages)
				{
					name = MarkdownFileBuilder.LinkString(item.Name, PathUtils.ToLink(item.FullName));
					var page = DocumentationPage.GeneratePage(this.OutputFolder, item);
					page.CreateFile();
				}
				else
				{
					name = item.Name;
				}

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

	public abstract class DocumentationPage<T> : DocumentationPage
	where T : CommonDocumentation
	{
		protected override string fileName { get { return PathUtils.ToLink(this._documentation.FullName); } }

		protected readonly T _documentation;

		protected DocumentationPage(string outputFolder, T documentation) : base(outputFolder)
		{
			this._documentation = documentation;
		}
	}
}