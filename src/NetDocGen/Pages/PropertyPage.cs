using NetDocGen.Extensions;

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

			builder.TextWithHeader(2, "Value", this._documentation.ValueDescription);
		}

		protected override void writeDefinition()
		{
			base.writeDefinition();

			builder.TextWithHeader(3, "Property Value", this._documentation.ReflectionInfo.PropertyType.GetMemberName());
		}
	}
}
