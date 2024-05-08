using System.Reflection;
using System.Xml.XPath;
using System.Xml;
using System.Xml.Linq;

namespace NetDocGen.Xml
{
	public class XmlParser
	{
		public bool UnIndentText { get; set; } = true;

		private readonly XPathNavigator _nav;

		// XML XPat
		private const string _assemblyXPath = "/doc/assembly";
		private const string _membersXPath = "/doc/members/member";
		private const string _memberXPath = "/doc/members/member[@name='{0}']";

		private const string _summaryXPath = "summary";
		private const string _remarksXPath = "remarks";
		private const string _exampleXPath = "example";
		private const string _valueXPath = "value";
		private const string _paramXPath = "param";
		private const string _typeParamXPath = "typeparam";
		private const string _responsesXPath = "response";
		private const string _returnsXPath = "returns";
		private const string _inheritdocXPath = "inheritdoc";

		//  XML attributes 
		private const string _nameAttribute = "name";
		private const string _codeAttribute = "code";
		private const string _crefAttribute = "cref";

		/// <summary>
		/// Create an Xml parser to explore the document
		/// </summary>
		/// <param name="path">Path to the Xml documentation file</param>
		public XmlParser(string path)
		{
			this._nav = new XPathDocument(path, XmlSpace.Preserve).CreateNavigator();
		}

		public AssemblyDocumentation ParseAssembly(Assembly assembly)
		{
			AssemblyDocumentation documentation = new AssemblyDocumentation(assembly);

			ParseAssembly(documentation);

			return documentation;
		}

		public void ParseAssembly(AssemblyDocumentation documentation)
		{
			foreach (NamespaceDocumentation ns in documentation.Namespaces)
			{
				foreach (TypeDocumentation t in ns.Types)
				{
					this.parseType(t.ReflectionInfo, t);
				}
			}
		}

		public TypeDocumentation ParseType(string fullname)
		{
			XPathNavigator node = getXmlMemberNode($"{XmlDocId.TypePrefix}:{fullname}");
			if (node == null)
				return null;

			TypeDocumentation documentation = new TypeDocumentation(fullname);
			this.getComments(node, documentation);

			XPathNodeIterator iterator = this._nav.Select($"/doc/members/member[@name[contains(.,':{fullname}')]]");
			foreach (XPathNavigator n in iterator)
			{
				CommonDocumentation doc;

				string prefixName = n.GetAttribute("name", "");
				string name = prefixName.Remove(0, 2);

				switch (XmlDocId.GetTypeByPrefix(prefixName.First()))
				{
					case MemberTypes.Constructor:
					case MemberTypes.Method:
						doc = new MethodDocumentation(name);
						documentation.AddMethod(doc as MethodDocumentation);
						break;
					case MemberTypes.Property:
						doc = new PropertyDocumentation(name);
						documentation.AddProperty(doc as PropertyDocumentation);
						break;
					case MemberTypes.Field:
					case MemberTypes.Event:
					case MemberTypes.TypeInfo:
					case MemberTypes.Custom:
					case MemberTypes.NestedType:
					case MemberTypes.All:
					default:
						continue;
				}

				this.getComments(n, doc);
			}

			return documentation;
		}

		public TypeDocumentation ParseType(Type type)
		{
			TypeDocumentation tdoc = new TypeDocumentation(type);
			this.parseType(type, tdoc);
			return tdoc;
		}

		private void parseType(Type type, TypeDocumentation tdoc)
		{
			this.getComments(type, tdoc);

			foreach (MethodDocumentation mdoc in tdoc.Methods)
			{
				this.getComments(mdoc.ReflectionInfo, mdoc);
			}

			foreach (PropertyDocumentation pdoc in tdoc.Properties)
			{
				this.getComments(pdoc.ReflectionInfo, pdoc);
			}
		}

		private string getNodePath(string name)
		{
			return string.Format(_memberXPath, name);
		}

		private XPathNavigator getXmlMemberNode(string name)
		{
			return this._nav.SelectSingleNode(this.getNodePath(name));
		}

		private void getComments(MemberInfo info, CommonDocumentation documentation)
		{
			XPathNavigator node = this.getXmlMemberNode(info.MemberId());

			if (node == null)
				return;

			this.getComments(node, documentation);
		}

		private void getComments(XPathNavigator node, CommonDocumentation documentation)
		{
			if (node == null)
				return;

			this.getCommonComments(node, documentation);

			switch (documentation)
			{
				case PropertyDocumentation propertyDocumentation:
					this.getPropertyComments(node, propertyDocumentation);
					break;
				case MethodDocumentation methodDocumentation:
					this.getMethodComments(node, methodDocumentation);
					break;
				default:
					break;
			}
		}

		private void getCommonComments(XPathNavigator rootNode, CommonDocumentation documentation)
		{
			var inheritNode = rootNode?.SelectSingleNode(_inheritdocXPath);
			if (inheritNode != null)
			{
				this.resolveInherit(documentation);
			}

			if (this.tryGetComment(rootNode, _summaryXPath, out string summary))
			{
				documentation.Summary = summary;
			}
			if (this.tryGetComment(rootNode, _remarksXPath, out string remarks))
			{
				documentation.Remarks = remarks;
			}
			if (this.tryGetComment(rootNode, _exampleXPath, out string example))
			{
				documentation.Example = example;
			}
		}

		private void resolveInherit(CommonDocumentation documentation)
		{
			//TODO: remove this switch, add a common interface
			switch (documentation)
			{
				case MethodDocumentation methodDocumentation:
					resolveInherit(methodDocumentation);
					break;
				case PropertyDocumentation propertyDocumentation:
					resolveInherit(propertyDocumentation);
					break;
			}
		}

		private void resolveInherit(MethodDocumentation documentation)
		{
			Type declaringType = documentation.ReflectionInfo.DeclaringType;
			foreach (var item in declaringType.GetInterfaces())
			{
				var p = item.GetMethod(documentation.Name);
				if (p != null)
				{
					this.getComments(p, documentation);
					break;
				}
			}
		}

		private void resolveInherit(PropertyDocumentation documentation)
		{
			Type declaringType = documentation.ReflectionInfo.DeclaringType;
			foreach (var item in declaringType.GetInterfaces())
			{
				var p = item.GetProperty(documentation.Name);
				if (p != null)
				{
					this.getComments(p, documentation);
					break;
				}
			}

			//TODO: Check base type
		}

		private void getPropertyComments(XPathNavigator rootNode, PropertyDocumentation documentation)
		{
			if (this.tryGetComment(rootNode, _valueXPath, out string valueDescription))
			{
				documentation.ValueDescription = valueDescription;
			}
		}

		private void getMethodComments(XPathNavigator rootNode, MethodDocumentation documentation)
		{
		}

		private bool tryGetComment(XPathNavigator rootNode, string name, out string value)
		{
			var node = rootNode?.SelectSingleNode(name);
			if (node != null)
			{
				value = this.getXmlText(node);
				return true;
			}
			else
			{
				value = null;
				return false;
			}
		}

		private string getXmlText(XPathNavigator node)
		{
			var innerText = node?.InnerXml ?? string.Empty;
			if (!UnIndentText || string.IsNullOrEmpty(innerText)) return innerText;

			var outerText = node?.OuterXml ?? string.Empty;
			var indentText = findIndent(outerText);
			if (string.IsNullOrEmpty(indentText)) return innerText;
			return innerText.Replace(indentText, indentText[0].ToString()).Trim('\r', '\n');
		}

		private string findIndent(string outerText)
		{
			if (string.IsNullOrEmpty(outerText)) return string.Empty;
			var end = outerText.LastIndexOf("</");
			if (end < 0) return string.Empty;
			var start = end - 1;
			for (; start >= 0 && outerText[start] != '\r' && outerText[start] != '\n'; start--) ;
			if (start < 0 || end <= start) return string.Empty;
			return outerText.Substring(start, end - start);
		}
	}
}