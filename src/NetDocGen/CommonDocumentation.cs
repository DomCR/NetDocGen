namespace NetDocGen
{
	public abstract class CommonDocumentation
	{
		public abstract string Name { get; }

		public abstract string FullName { get; }

		public string Summary { get; set; }

		public string Remarks { get; set; }

		public string Example { get; set; }

		protected CommonDocumentation()
		{
			this.Summary = string.Empty;
			this.Remarks = string.Empty;
			this.Example = string.Empty;
		}

		public abstract AssemblyDocumentation GetRoot();

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{FullName}";
		}
	}
}
