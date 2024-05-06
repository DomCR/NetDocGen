using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using NetDocGen.Runner.Commands;

namespace NetDocGen.Runner
{
	internal class Program
	{
		static int Main(string[] args)
		{
			try
			{
				var services = new ServiceCollection()
				.RegisterDependencies()
				.BuildServiceProvider();

				using (var cli = new CommandLineApplication<MarkdownWikiCommand>())
				{
					cli.Conventions.UseDefaultConventions()
						.UseConstructorInjection(services);

					return cli.Execute(args);
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine(ex.Message);
				Console.ResetColor();
				return 1;
			}
		}
	}
}
