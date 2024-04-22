namespace NetDocGen.Utils
{
	public static class PathUtils
	{
		public static string ToLink(string name)
		{
			return string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
		}
	}
}
