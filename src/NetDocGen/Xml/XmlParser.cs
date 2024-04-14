using System.Linq;
using System.Reflection;
using System.Xml.XPath;
using System.Xml;

namespace NetDocGen.Xml
{
	public class XmlParser : IDisposable
	{
		[Obsolete]
		public Assembly Assembly { get; private set; }

		private readonly XPathNavigator _nav;

		// XML XPat
		private const string assemblyXPath = "/doc/assembly";
		private const string membersXPath = "/doc/members/member";
		private const string _memberXPath = "/doc/members/member[@name='{0}']";
		private const string _summaryXPath = "summary";
		private const string _remarksXPath = "remarks";
		private const string _exampleXPath = "example";
		private const string _paramXPath = "param";
		private const string _typeParamXPath = "typeparam";
		private const string _responsesXPath = "response";
		private const string _returnsXPath = "returns";
		private const string _inheritdocXPath = "inheritdoc";

		//  XML attributes 
		private const string nameAttribute = "name";
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

		public AssemblyDocumentation ParseAssembly()
		{
			XPathNavigator root = this._nav.SelectSingleNode(assemblyXPath);
			var documentation = new AssemblyDocumentation(root.SelectSingleNode(nameAttribute).InnerXml);

			parseXmlTypes(documentation);

			return documentation;
		}

		private void parseXmlTypes(AssemblyDocumentation documentation)
		{
			HashSet<string> ids = new HashSet<string>();

			XPathNodeIterator types = this._nav.Select($"/doc/members/member[@name[contains(.,'T:')]]");

			IEnumerable<XPathNavigator> sorted =
				 from XPathNavigator nav in types
				 orderby nav.GetAttribute("name", string.Empty)
				 select nav;

			foreach (XPathNavigator m in sorted)
			{
				string value = m.GetAttribute("name", string.Empty);
				char prefix = value.Split(":").First()[0];
				string fullname = value.Split(":").Last();

				Console.WriteLine(fullname);
			}
		}

		public AssemblyDocumentation ParseAssembly(Assembly assembly)
		{
			AssemblyDocumentation documentation = new AssemblyDocumentation(assembly);

			foreach (Type t in assembly.ExportedTypes)
			{
				if (!documentation.TryGetNamespace(t.Namespace, out NamespaceDocumentation ns))
				{
					ns = new NamespaceDocumentation(t.Namespace, documentation);
					documentation.AddNamespace(ns);
				}

				TypeDocumentation tdoc = this.parseType(t, ns);
				ns.Types.Add(tdoc);
			}

			return documentation;
		}

		public void ParseAssembly(AssemblyDocumentation documentation)
		{
			foreach (NamespaceDocumentation ns in documentation.Namespaces)
			{

			}
		}

		public TypeDocumentation ParseType(string fullname)
		{
			XPathNavigator node = getXmlMemberNode($"{XmlDocId.TypePrefix}:{fullname}");
			if (node == null)
				return null;

			TypeDocumentation documentation = new TypeDocumentation(fullname);
			this.getComments(node, documentation);

			XPathExpression exp;    //Check expressions
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
						documentation.Methods.Add(doc as MethodDocumentation);
						break;
					case MemberTypes.Property:
						doc = new PropertyDocumentation(name);
						documentation.Properties.Add(doc as PropertyDocumentation);
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
			return this.parseType(type, null);
		}

		/// <inheritdoc/>
		public void Dispose()
		{
		}

		private TypeDocumentation parseType(Type type, NamespaceDocumentation parent)
		{
			TypeDocumentation tdoc = new TypeDocumentation(type, parent);
			this.getComments(type, tdoc);

			foreach (MethodInfo m in type.GetMethods(BindingFlags.Public
														| BindingFlags.Instance
														| BindingFlags.DeclaredOnly))
			{
				if (m.IsSpecialName)
					continue;

				MethodDocumentation mdoc = new MethodDocumentation(m, tdoc);
				this.getComments(m, mdoc);
				tdoc.Methods.Add(mdoc);
			}

			foreach (PropertyInfo p in type.GetProperties(BindingFlags.Public
														| BindingFlags.Instance
														| BindingFlags.DeclaredOnly))
			{
				PropertyDocumentation pdoc = new PropertyDocumentation(p, tdoc);
				this.getComments(p, pdoc);
				tdoc.Properties.Add(pdoc);
			}

			return tdoc;
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
			//comments = ResolveInheritdocComments(comments, type);
			switch (documentation)
			{
				case MethodDocumentation methodDocumentation:
					this.getMethodComments(node, methodDocumentation);
					break;
				default:
					break;
			}
		}

		private void getCommonComments(XPathNavigator rootNode, CommonDocumentation documentation)
		{
			documentation.Summary = this.getComment(rootNode, _summaryXPath);
			documentation.Remarks = this.getComment(rootNode, _remarksXPath);
			documentation.Example = this.getComment(rootNode, _exampleXPath);
			//documentation.Inheritdoc = GetInheritdocTag(rootNode);
		}

		private void getMethodComments(XPathNavigator rootNode, MethodDocumentation documentation)
		{
		}

		private string getComment(XPathNavigator rootNode, string name)
		{
			return this.getXmlText(rootNode?.SelectSingleNode(name));
		}

		private string getXmlText(XPathNavigator node)
		{
			return node?.InnerXml ?? string.Empty;
		}
	}
}
