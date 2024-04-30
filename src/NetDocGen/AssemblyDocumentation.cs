using CSUtilities.Extensions;
using NetDocGen.Xml;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NetDocGen
{
	public class AssemblyDocumentation : CommonDocumentation
	{
		public override string Name { get; }

		public override string FullName { get { return this.Name; } }

		public string Version { get; private set; }

		public IEnumerable<NamespaceDocumentation> Namespaces
		{
			get
			{
				return this._namespaces.Values;
			}
		}

		private Assembly _assembly;

		private readonly Dictionary<string, NamespaceDocumentation> _namespaces = new();

		public AssemblyDocumentation(Assembly assembly)
		{
			this._assembly = assembly;

			this.Name = Path.GetFileNameWithoutExtension(this._assembly.Location);

			this.Summary = this._assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
			this.Version = this._assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+').FirstOrDefault();
			if (this.Version.IsNullOrEmpty())
			{
				this.Version = this._assembly.GetName().Version.ToString();
			}

			this.processAssembly();
		}

		public override AssemblyDocumentation GetRoot()
		{
			return this;
		}

		public void AddNamespace(NamespaceDocumentation ns)
		{
			this._namespaces.Add(ns.FullName, ns);
		}

		public NamespaceDocumentation GetNamespace(string name)
		{
			return this._namespaces[name];
		}

		public bool TryGetNamespace(string name, out NamespaceDocumentation ns)
		{
			return this._namespaces.TryGetValue((string)name, out ns);
		}

		public void UpdateComments(string path)
		{
			using (XmlParser parser = new XmlParser(path))
			{
				parser.ParseAssembly(this);
			}
		}

		private void processAssembly()
		{
			foreach (Type t in this._assembly.ExportedTypes)
			{
				if (t.IsNested)
				{
					continue;
				}

				if (!this._namespaces.TryGetValue(t.Namespace, out NamespaceDocumentation ns))
				{
					ns = new NamespaceDocumentation(t.Namespace, this);
					this._namespaces.Add(t.Namespace, ns);
				}

				if (t.IsEnum)
				{

				}

				if (t.IsInterface)
				{

				}

				TypeDocumentation tdoc = new(t, ns);
				ns.Types.Add(tdoc);
			}
		}
	}
}