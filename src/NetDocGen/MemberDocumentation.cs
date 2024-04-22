using System.Linq;
using System.Reflection;

namespace NetDocGen
{
	public class MemberDocumentation<T, R> : CommonDocumentation
		where T : MemberInfo
		where R : CommonDocumentation
	{
		public override string Name { get; }

		public override string FullName { get { return this._fullName; } }

		public R Owner { get; }

		public T ReflectionInfo { get; }

		protected string _fullName;

		public MemberDocumentation(string fullname)
		{
			this._fullName = fullname;
			this.Name = fullname.Split('.').Last();
		}

		public MemberDocumentation(T info)
		{
			this.ReflectionInfo = info;
			this.Name = this.removeInvalidCharacters(this.ReflectionInfo.Name);

			if (this.ReflectionInfo.DeclaringType != null)
			{
				string fname = $"{this.ReflectionInfo.DeclaringType.FullName}.{this.Name}";
				this._fullName = this.removeInvalidCharacters(fname);
			}
		}

		public MemberDocumentation(T info, R owner) : this(info)
		{
			this.Owner = owner;
		}

		public override AssemblyDocumentation GetRoot()
		{
			return this.Owner?.GetRoot();
		}

		protected string removeInvalidCharacters(string name)
		{
			return name.Replace("`1", "<T>");
		}
	}
}