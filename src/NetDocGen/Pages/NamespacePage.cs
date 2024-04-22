using NetDocGen.Utils;

namespace NetDocGen.Pages
{
	public class NamespacePage : DocumentationPage
	{
		protected override string title { get { return $"{this._documentation.FullName} Namespace"; } }

		protected override string fileName { get { return PathUtils.ToLink(this._documentation.FullName); } }

		private readonly NamespaceDocumentation _documentation;

		public NamespacePage(NamespaceDocumentation documentation, string outputFolder) : base(outputFolder)
		{
			this._documentation = documentation;
		}

		protected override void build()
		{
			foreach (TypeDocumentation t in _documentation.Types.OrderBy(t => t.Name))
			{
				TypePage tpage = new TypePage(t, OutputFolder);
				tpage.Create();
			}

			this.buildDataTable(2, "Classes", _documentation.Types.OrderBy(t => t.Name), true);
		}
	}
}
