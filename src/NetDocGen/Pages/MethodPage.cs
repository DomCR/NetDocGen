namespace NetDocGen.Pages
{
	public class MethodPage : MemberPage<MethodDocumentation>
	{
		protected override string memberName => "Method";

		public MethodPage(string outputFolder, MethodDocumentation documentation) : base(outputFolder, documentation)
		{
		}
	}
}
