namespace MockAssembly
{
	/// <summary>
	/// This is a documented interface
	/// </summary>
	public interface IMockInterface
	{
		/// <summary>
		/// Base summary property from an interface
		/// </summary>
		/// <remarks>
		/// Base remarks property from an interface
		/// </remarks>
		string BaseProperty { get; set; }

		/// <summary>
		/// Base summary method from an interface
		/// </summary>
		/// <remarks>
		/// Base remarks method from an interface
		/// </remarks>
		void BaseMethodInInterface();
	}
}