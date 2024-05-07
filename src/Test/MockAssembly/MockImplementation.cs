namespace MockAssembly
{
	/// <summary>
	/// This is a class that impmlements an int
	/// </summary>
	public class MockImplementation : IMockInterface
	{
		/// <inheritdoc/>
		public string BaseProperty { get; set; }
	}
}
