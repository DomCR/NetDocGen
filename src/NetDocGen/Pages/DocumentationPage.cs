using NetDocGen.Extensions;
using NetDocGen.Markdown;
using NetDocGen.Utils;
using System.Reflection;
using System.Text;

namespace NetDocGen.Pages
{
	public abstract class DocumentationPage
	{
		public abstract string Title { get; }

		protected abstract string FileName { get; }

		public string OutputFolder { get; protected set; }

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

			this.builder.Header(1, this.Title);

			this.build();

			output.Append(this.builder.ToString());
		}

		public static DocumentationPage GeneratePage<T>(string outputFolder, T documentation)
			where T : ICommonDocumentation
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
			string file = this.FileName;

			if (Path.GetExtension(file) != "md")
			{
				file = $"{FileName}.md";
			}

			return Path.Combine(OutputFolder, file);
		}

		protected void buildDataTable<T, R>(int level, string title, IEnumerable<T> data, bool createSubpages = false)
			where T : IMemberDocumentation<R>
			where R : MemberInfo
		{
			if (!data.Any())
			{
				return;
			}

			builder.Header(level, title);

			bool isType = typeof(R) == typeof(Type);
			string[] cols = null;
			string[] alignment = null;
			if (isType)
			{
				cols = new string[] { "Name", "Summary" };
				alignment = new string[] { ":-", ":-" };
			}
			else
			{
				cols = new string[] { "Returns", "Name", "Summary" };
				alignment = new string[] { ":-:", ":-", ":-" };
			}

			List<List<string>> rows = new();
			foreach (T item in data.OrderBy(a => a.Name))
			{
				List<string> elements = new List<string>();

				if (!isType)
				{
					var t = item.ReflectionInfo.GetReturningType();
					elements.Add($"`{t.GetMemberName()}`");
				}

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

				elements.Add(name);
				elements.Add(item.Summary);

				rows.Add(elements);
			}

			this.builder.Table(
				cols,
				alignment: alignment,
				items: rows);
		}
	}

	public abstract class DocumentationPage<T> : DocumentationPage
		where T : CommonDocumentation
	{
		protected override string FileName { get { return PathUtils.ToLink(this._documentation.FullName); } }

		protected readonly T _documentation;

		protected DocumentationPage(string outputFolder, T documentation) : base(outputFolder)
		{
			this._documentation = documentation;
		}
	}
}