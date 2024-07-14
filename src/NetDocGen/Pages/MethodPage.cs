using NetDocGen.Extensions;

namespace NetDocGen.Pages
{
	public class MethodPage : MemberPage<MethodDocumentation>
	{
		protected override string memberName => "Method";

		public MethodPage(string outputFolder, MethodDocumentation documentation) : base(outputFolder, documentation)
		{
		}

		protected override void writeDefinition()
		{
			base.writeDefinition();

			//TODO: Write Parameters
			//if (this._documentation.Parameters.Any())
			//{
			//	builder.Header(3, "Parameters");
			//}

			builder.TextWithHeader(3, "Return Value", this._documentation.ReflectionInfo.ReturnType.GetMemberName());
		}
	}
}
