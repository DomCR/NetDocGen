namespace NetDocGen.Pages
{
	public class TypePage : DocumentationPage
	{
		private readonly TypeDocumentation _documentation;

		public TypePage(TypeDocumentation documentaiton, string outputFolder) : base(documentaiton.Name, outputFolder)
		{
			_documentation = documentaiton;
		}

		protected override void build()
		{
			builder.AppendLine(_documentation.Summary);

			buildDataTable("Properties", this._documentation.Properties);
			buildDataTable("Methods", this._documentation.Methods);
		}

		private void buildDataTable(string title, IEnumerable<CommonDocumentation> data)
		{
			builder.Header(2, title);

			string[] cols = new string[] { "Name", "Summary" };

			List<List<string>> rows = new();
			foreach (CommonDocumentation item in data)
			{
				rows.Add(new List<string>()
				{
					item.Name,
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
