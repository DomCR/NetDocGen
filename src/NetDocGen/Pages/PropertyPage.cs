namespace NetDocGen.Pages
{
	public abstract class MemberPage<T> : DocumentationPage<T>
		where T : CommonDocumentation
	{
		protected abstract string memberName { get; }

		protected override string title { get { return $"{this._documentation.Name} {memberName}"; } }

		protected MemberPage(string outputFolder, T documentation) : base(outputFolder, documentation)
		{
		}

		protected override void build()
		{
			builder.AppendLine(_documentation.Summary);

			builder.TextWithHeader(2, "Remarks", _documentation.Remarks);
		}
	}

	public class PropertyPage : MemberPage<PropertyDocumentation>
	{
		protected override string memberName => "Property";

		public PropertyPage(string outputFolder, PropertyDocumentation documentation) : base(outputFolder, documentation)
		{
		}

		protected override void build()
		{
			base.build();

			this.writeDefinition();

			builder.TextWithHeader(2, "Value", this._documentation.ValueDescription);
		}

		protected override void writeDefinition()
		{
			base.writeDefinition();
		}
	}
	public class MethodPage : MemberPage<MethodDocumentation>
	{
		protected override string memberName => "Method";

		public MethodPage(string outputFolder, MethodDocumentation documentation) : base(outputFolder, documentation)
		{
		}
	}
}
