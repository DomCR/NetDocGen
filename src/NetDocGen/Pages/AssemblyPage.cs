namespace NetDocGen.Pages
{
	public class AssemblyPage : DocumentationPage
	{
		protected override string title { get { return this._documentation.Name; } }

		protected override string fileName { get { return "Home"; } }

		private readonly AssemblyDocumentation _documentation;

		public AssemblyPage(AssemblyDocumentation documentation, string folder) : base(folder)
		{
			this._documentation = documentation;
		}

		protected override void build()
		{
			this.builder.AppendLine($"{_documentation.Name} {_documentation.Version} Library");

			this.builder.AppendLine(_documentation.Summary);

			this.builder.Header(2, $"Namespaces");

			foreach (NamespaceDocumentation ns in this._documentation.Namespaces.OrderBy(n => n.FullName))
			{
				NamespacePage nsPage = new NamespacePage(ns, OutputFolder);
				nsPage.CreateFile();

				this.builder.Header(2, $"{ns.FullName} Namespace", ns.FullName);

				this.buildDataTable(3, "Classes", ns.Types.OrderBy(t => t.Name), true);

				this.builder.AppendLine();
			}

			string str = this.builder.ToString();
		}
	}
}
