using NetDocGen.Extensions;
using NetDocGen.Markdown;
using NetDocGen.Utils;

namespace NetDocGen.Pages
{
	public abstract class MemberPage<T> : DocumentationPage<T>
		where T : MemberDocumentation
	{
		protected abstract string memberName { get; }

		protected override string Title { get { return $"{this._documentation.Name} {memberName}"; } }

		protected MemberPage(string outputFolder, T documentation) : base(outputFolder, documentation)
		{
		}

		protected override void build()
		{
			builder.AppendLine(_documentation.Summary);

			builder.TextWithHeader(2, "Remarks", _documentation.Remarks);

			this.writeDefinition();
		}

		protected virtual void writeDefinition()
		{
			builder.Header(2, "Definition");

			builder.Append("Namespace:", MarkdownTextStyle.Bold);
			string ns = MarkdownFileBuilder.LinkString(
				_documentation.GetOwner().FullName,
				PathUtils.ToLink(_documentation.GetOwner().FullName));
			builder.AppendLine($" {ns}");

			builder.AppendLine("C#", MarkdownTextStyle.Bold);

			builder.Code(_documentation.GetMemberInfo().GetSignature(), "C#");
		}
	}
}
