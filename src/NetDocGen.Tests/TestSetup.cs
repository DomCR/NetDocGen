using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestFramework("NetDocGen.Tests.TestSetup", "NetDocGen.Tests")]

namespace NetDocGen.Tests
{
	public sealed class TestSetup : XunitTestFramework
	{
		public TestSetup(IMessageSink messageSink)
		  : base(messageSink)
		{
			this.init();
		}

		private void init()
		{
			TestVariables.CreateOutputFolders();
		}
	}
}