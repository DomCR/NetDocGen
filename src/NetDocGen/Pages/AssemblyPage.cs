namespace NetDocGen.Pages
{
	public class AssemblyPage : DocumentationPage<AssemblyDocumentation>
	{
		public override string Title { get { return this._documentation.Name; } }

		protected override string FileName { get { return "Home"; } }

		public AssemblyPage(string folder, AssemblyDocumentation documentation) : base(folder, documentation)
		{
		}

		protected override void build()
		{
			this.builder.AppendLine($"{this._documentation.Name} {this._documentation.Version} Library");

			this.builder.AppendLine(this._documentation.Summary);

			this.builder.Header(2, $"Namespaces");

			foreach (NamespaceDocumentation ns in this._documentation.Namespaces.OrderBy(n => n.FullName))
			{
				NamespacePage nsPage = new NamespacePage(this.OutputFolder, ns);
				nsPage.CreateFile();

				this.builder.Header(2, $"{ns.FullName} Namespace", ns.FullName);

				this.buildDataTable<TypeDocumentation, Type>(3, "Classes", ns.Types, true);

				this.builder.AppendLine();
			}

			string str = this.builder.ToString();
		}
	}
}
