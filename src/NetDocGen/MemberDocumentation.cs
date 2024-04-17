using System.Linq;
using System.Reflection;

namespace NetDocGen
{
	public class MemberDocumentation<T, R> : CommonDocumentation
		where T : MemberInfo
		where R : CommonDocumentation
	{
		public override string Name { get; }

		public override string FullName { get; }

		public R Owner { get; }

		protected T _info;

		public MemberDocumentation(string fullname)
		{
			this.FullName = fullname;
			this.Name = fullname.Split('.').Last();
		}

		public MemberDocumentation(T info)
		{
			this._info = info;
			this.Name = this.removeInvalidCharacters($"{this._info.Name}");

			if (this._info.DeclaringType != null)
				this.FullName = $"{this._info.DeclaringType.FullName}.{this.Name}";
		}

		public MemberDocumentation(T info, R owner) : this(info)
		{
			this.Owner = owner;
		}

		public override AssemblyDocumentation GetRoot()
		{
			return this.Owner?.GetRoot();
		}

		private string removeInvalidCharacters(string name)
		{
			return name.Replace("`1", "<T>");
		}
	}
}