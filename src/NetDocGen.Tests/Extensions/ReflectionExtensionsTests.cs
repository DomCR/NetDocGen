using MockAssembly;
using NetDocGen.Extensions;
using System.Reflection;

namespace NetDocGen.Tests.Extensions
{
	public class ReflectionExtensionsTests
	{
		[Fact]
		public void GetConstructorName()
		{
			var m = typeof(MockClass).GetConstructor(new Type[] { });

			string name = m.GetMethodName();
			string fullname = m.GetMethodFullName();

			Assert.NotNull(m);

			Assert.Equal("ctor", name);
			Assert.Equal("MockAssembly.MockClass.ctor", fullname);
		}

		[Fact]
		public void GetConstructorNameWithParameter()
		{
			var m = typeof(MockClass).GetConstructor(new Type[] { typeof(string) });

			Assert.NotNull(m);

			string name = m.GetMethodName();
			string fullname = m.GetMethodFullName();

			Assert.Equal("ctor(System.String)", name);
			Assert.Equal("MockAssembly.MockClass.ctor(System.String)", fullname);
		}

		[Fact]
		public void GetPropertyName()
		{
			PropertyInfo? m = typeof(MockClass).GetProperty(nameof(MockClass.Name));

			Assert.NotNull(m);

			string name = m.Name;
			string fullname = m.GetPropertyFullName();

			Assert.Equal("Name", name);
			Assert.Equal("MockAssembly.MockClass.Name", fullname);
		}

		[Fact]
		public void GetMethodNameNoParameters()
		{
			var m = typeof(MockClass).GetMethod(nameof(MockClass.Hello), new Type[] { });

			Assert.NotNull(m);
			
			string name = m.GetMethodName();
			string fullname = m.GetMethodFullName();

						Assert.Equal("Hello", name);
			Assert.Equal("MockAssembly.MockClass.Hello", fullname);
		}

		[Fact]
		public void GetMethodNameParameters()
		{
			var m = typeof(MockClass).GetMethod(nameof(MockClass.Hello), new Type[] { typeof(string) });

			Assert.NotNull(m);
			
			string name = m.GetMethodName();
			string fullname = m.GetMethodFullName();

			Assert.Equal("Hello(System.String)", name);
			Assert.Equal("MockAssembly.MockClass.Hello(System.String)", fullname);
		}

		[Fact]
		public void GetMethodNameMultipleParameters()
		{
			var m = typeof(MockClass).GetMethod(nameof(MockClass.Hello), new Type[] { typeof(string), typeof(string) });

			Assert.NotNull(m);

			string name = m.GetMethodName();
			string fullname = m.GetMethodFullName();

			Assert.Equal("Hello(System.String,System.String)", name);
			Assert.Equal("MockAssembly.MockClass.Hello(System.String,System.String)", fullname);
		}

		[Fact]
		public void GetMethodWithGeneric()
		{
			var m = typeof(MockClass).GetMethod(nameof(MockClass.MethodWithGeneric), new Type[] { });

			Assert.NotNull(m);

			string name = m.GetMethodName();
			string fullname = m.GetMethodFullName();

			Assert.Equal("MethodWithGeneric<T>", name);
			Assert.Equal("MockAssembly.MockClass.MethodWithGeneric<T>", fullname);
		}

		[Fact]
		public void GetMethodWithMultipleGeneric()
		{
			var m = typeof(MockClass).GetMethod(nameof(MockClass.MethodWithMultipleGeneric), new Type[] { });

			Assert.NotNull(m);

			string name = m.GetMethodName();
			string fullname = m.GetMethodFullName();

			Assert.Equal("MethodWithMultipleGeneric<T,R>", name);
			Assert.Equal("MockAssembly.MockClass.MethodWithMultipleGeneric<T,R>", fullname);
		}
	}
}
