using System.Reflection;

namespace NetDocGen
{
	public interface IMemberDocumentation<T> : ICommonDocumentation
		where T : MemberInfo
	{
		public T ReflectionInfo { get; }
	}
}