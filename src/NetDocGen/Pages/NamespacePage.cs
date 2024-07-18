namespace NetDocGen.Pages
{
	public class NamespacePage : DocumentationPage<NamespaceDocumentation>
	{
		protected override string Title { get { return $"{this._documentation.FullName} Namespace"; } }

		public NamespacePage(string outputFolder, NamespaceDocumentation documentation) : base(outputFolder, documentation)
		{
		}

		protected override void build()
		{
			foreach (TypeDocumentation t in _documentation.Types.OrderBy(t => t.Name))
			{
				TypePage tpage = new TypePage(OutputFolder, t);
				tpage.CreateFile();
			}

			this.buildDataTable<TypeDocumentation, Type>(2, "Classes", _documentation.Types, true);
		}
	}
}
