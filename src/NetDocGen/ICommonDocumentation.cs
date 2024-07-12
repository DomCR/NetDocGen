namespace NetDocGen
{
	public interface ICommonDocumentation
	{
		public string Name { get; }

		public string FullName { get; }

		public string Summary { get; set; }

		public string Remarks { get; set; }

		public string Example { get; set; }
	}
}
