﻿namespace NetDocGen.Pages
{
	public class PropertyPage : DocumentationPage<PropertyDocumentation>
	{
		protected override string title { get { return $"{this._documentation.Name} Class"; } }

		public PropertyPage(string outputFolder, PropertyDocumentation documentation) : base(outputFolder, documentation)
		{
		}

		protected override void build()
		{
			builder.AppendLine(_documentation.Summary);

			builder.TextWithHeader(2, "Remarks", _documentation.Remarks);
		}
	}
}