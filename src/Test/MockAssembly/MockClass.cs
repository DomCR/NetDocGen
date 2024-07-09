﻿using System;

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

		public int ZeroMethod()
		{
			return 0;
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
