namespace NetDocGen.Pages
{
	public class PropertyPage : MemberPage<PropertyDocumentation>
	{
		protected override string memberName => "Property";

		public PropertyPage(string outputFolder, PropertyDocumentation documentation) : base(outputFolder, documentation)
		{
		}

		protected override void build()
		{
			base.build();

			//TODO: this.writeDefinition();

			builder.TextWithHeader(2, "Value", this._documentation.ValueDescription);
		}

		protected override void writeDefinition()
		{
			base.writeDefinition();
		}
	}
}
