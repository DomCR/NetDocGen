using NetDocGen.Extensions;
using System.Reflection;

namespace NetDocGen
{
	public abstract class MemberDocumentation : CommonDocumentation
	{
	}

	public abstract class MemberDocumentation<T, R> : MemberDocumentation
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
			this.Name = info.GetMemberName();
			this._fullName = info.GetMemberFullName();
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