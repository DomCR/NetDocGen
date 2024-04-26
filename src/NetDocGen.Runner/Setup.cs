using Microsoft.Extensions.DependencyInjection;
using NetDocGen.Services;

namespace NetDocGen.Runner
{
	public static class Setup
	{
		public static IServiceCollection RegisterDependencies(this IServiceCollection serviceCollection)
		{
			return serviceCollection.AddTransient<IMarkDownGenerator, MarkDownWikiGenerator>();
		}
	}
}
