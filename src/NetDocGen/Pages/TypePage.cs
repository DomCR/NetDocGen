using NetDocGen.Utils;

namespace NetDocGen.Pages
{
	public class TypePage : DocumentationPage
	{
		protected override string title { get { return $"{this._documentation.Name} Class"; } }

		protected override string fileName { get { return PathUtils.ToLink(this._documentation.FullName); } }

		private readonly TypeDocumentation _documentation;

		public TypePage(TypeDocumentation documentaiton, string outputFolder) : base(outputFolder)
		{
			_documentation = documentaiton;
		}

		protected override void build()
		{
			builder.AppendLine(_documentation.Summary);

			buildDataTable(2, "Properties", this._documentation.Properties);
			buildDataTable(2, "Methods", this._documentation.Methods);
		}
	}
}
