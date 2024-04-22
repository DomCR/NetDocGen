using Microsoft.Extensions.DependencyInjection;
using NetDocGen.Services;

namespace NetDocGen.Runner
{
	public static class Setup
	{
		public static void RegisterDependencies(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddTransient<IMarkDownGenerator, MarkDownWikiGenerator>();
		}
	}
}
