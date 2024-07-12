using NetDocGen.Extensions;
using NetDocGen.Markdown;
using NetDocGen.Utils;
using System.Reflection;
using System.Xml.Linq;

namespace NetDocGen.Pages
{
	public class TypePage : DocumentationPage<TypeDocumentation>
	{
		protected override string title { get { return $"{this._documentation.Name} Class"; } }

		public TypePage(string outputFolder, TypeDocumentation documentaiton) : base(outputFolder, documentaiton)
		{
		}

		protected override void build()
		{
			builder.AppendLine(_documentation.Summary);

			builder.TextWithHeader(2, "Remarks", _documentation.Remarks);

			//TODO: Definition
			//this.writeDefinition();

			//Inheritance

			//Derived -> optional

			buildDataTable<PropertyDocumentation, PropertyInfo>(2, "Properties", this._documentation.Properties, true);
			buildDataTable<MethodDocumentation, MethodInfo>(2, "Methods", this._documentation.Methods, true);
			buildDataTable<EventDocumentation, EventInfo>(2, "Events", this._documentation.Events, true);

			//Events
		}

		protected override void writeDefinition()
		{
			this.writeDefinition();

			builder.Append("Namespace:", MarkdownTextStyle.Bold);
			string ns = MarkdownFileBuilder.LinkString(_documentation.Owner.FullName, PathUtils.ToLink(_documentation.Owner.FullName));
			builder.AppendLine($" {ns}");

			builder.AppendLine("C#", MarkdownTextStyle.Bold);

			this._documentation.ReflectionInfo.GetSignature();

			builder.Code($"class {this._documentation.Name}", "C#");
		}
	}
}
