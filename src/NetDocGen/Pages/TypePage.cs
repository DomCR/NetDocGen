using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDocGen.Pages
{
	public class TypePage : DocumentationPage
	{
		private readonly TypeDocumentation _documentation;

		public TypePage(TypeDocumentation documentaiton, string outputFolder) : base(documentaiton.Name, outputFolder)
		{
			_documentation = documentaiton;
		}

		protected override void build()
		{
			builder.AppendLine(_documentation.Summary);

			builder.Header(2, "Properties");

			List<List<string>> properties = new();
			foreach (var item in _documentation.Properties)
			{
				List<string> cols = new()
				{
					item.Name,
					item.Summary
				};

				properties.Add(cols);
			}

			builder.Table(
				new string[] { "Name", "Summary" }, 
				alignment: new string[] { ":-", ":-" },
				items: properties);
		}
	}
}
