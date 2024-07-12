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
}
