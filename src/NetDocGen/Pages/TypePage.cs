using System.Reflection;

namespace NetDocGen.Pages
{
	public class TypePage : MemberPage<TypeDocumentation>
	{
		protected override string memberName { get { return "Class"; } }

		public TypePage(string outputFolder, TypeDocumentation documentaiton) : base(outputFolder, documentaiton)
		{
		}

		protected override void build()
		{
			base.build();

			//Inheritance

			//Derived -> optional

			buildDataTable<PropertyDocumentation, PropertyInfo>(2, "Properties", this._documentation.Properties, true);
			buildDataTable<MethodDocumentation, MethodInfo>(2, "Methods", this._documentation.Methods, true);
			buildDataTable<EventDocumentation, EventInfo>(2, "Events", this._documentation.Events, true);

			//Events
		}
	}
}
