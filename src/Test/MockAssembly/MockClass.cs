using System;

namespace MockAssembly
{
	/// <summary>
	/// Summary for a mock class
	/// </summary>
	/// <remarks>
	/// Remarks text for mock class
	/// </remarks>
	public class MockClass
	{
		/// <summary>
		/// Class event summary
		/// </summary>
		public event EventHandler PropertyChanged;

		/// <summary>
		/// This is a public property
		/// </summary>
		/// <remarks>
		/// This is a remarks tag value for a public property
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// This is a public field
		/// </summary>
		public string Field;

		/// <summary>
		/// This is a public property with a value description
		/// </summary>
		/// <value>Value cannot be less than 0</value>
		public int ConstrainValue { get; set; }

		/// <summary>
		/// This is a protected property
		/// </summary>
		protected string protectedSecret { get; set; }

		private string _mySecret;

		/// <summary>
		/// Default empty constructor
		/// </summary>
		public MockClass()
		{
		}

		/// <summary>
		/// Constructor with one parameter
		/// </summary>
		/// <param name="name">parameter description here</param>
		public MockClass(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// This is a summary for a Hello method
		/// </summary>
		public void Hello()
		{
		}

		/// <summary>
		/// Overload method with a parameter.
		/// </summary>
		/// <param name="param">random parameter.</param>
		public void Hello(string param)
		{
		}

		/// <summary>
		/// Overload method with 2 parameters
		/// </summary>
		/// <param name="param1"></param>
		/// <param name="param2"></param>
		public void Hello(string param1, string param2)
		{
		}

		/// <summary>
		/// This one returns an int.
		/// </summary>
		/// <returns>returns always 0.</returns>
		public int ZeroMethod()
		{
			return 0;
		}

		public MockClass Create()
		{
			return new MockClass();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void MethodWithGeneric<T>()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="R"></typeparam>
		public void MethodWithMultipleGeneric<T, R>()
		{

		}

		public void MethodWithGenericParam<T>(T param)
		{
		}

		protected void myProtectedMethod()
		{
		}

		internal void myInternalMethod()
		{
		}

		private void myPrivateMethod()
		{
		}
	}
}
