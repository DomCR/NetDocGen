﻿using MockAssembly;
using NetDocGen.Xml;

namespace NetDocGen.Tests.Xml
{
	public class XmlParserTests
	{
		private static readonly XmlParser _instance;

		static XmlParserTests()
		{
			_instance = new XmlParser(Path.Combine(TestVariables.SamplesFolder, "MockAssembly.xml"));
		}

		[Fact]
		public void ParseTypeTest()
		{
			TypeDocumentation doc = _instance.ParseType(typeof(MockClass));

			Assert.NotNull(doc);

			Assert.Equal("Summary for a mock class", doc.Summary);
			Assert.Equal("Remarks text for mock class", doc.Remarks);
		}

		[Fact]
		public void ParseTypePropertiesTest()
		{
			TypeDocumentation doc = _instance.ParseType(typeof(MockClass));

			Assert.NotNull(doc);

			Assert.NotEmpty(doc.Properties);

			var pname = doc.GetProperty(nameof(MockClass.Name));
			Assert.Equal("This is a public property", pname.Summary);
			Assert.Equal("This is a remarks tag value for a public property", pname.Remarks);

			var pconstrain = doc.GetProperty(nameof(MockClass.ConstrainValue));
			Assert.Equal("This is a public property with a value description", pconstrain.Summary);
			Assert.Equal("Value cannot be less than 0", pconstrain.ValueDescription);
		}

		[Fact]
		public void IneritDocTagTest()
		{
			TypeDocumentation doc = _instance.ParseType(typeof(MockImplementation));

			Assert.NotNull(doc);

			var pdoc = doc.GetProperty(nameof(MockImplementation.BaseProperty));
			Assert.Equal("Base summary property from an interface", pdoc.Summary);
			Assert.Equal("Base remarks property from an interface", pdoc.Remarks);

			var mdoc = doc.GetMethod(nameof(MockImplementation.BaseMethodInInterface));
			Assert.Equal("Base summary method from an interface", mdoc.Summary);
			Assert.Equal("Base remarks method from an interface", mdoc.Remarks);
		}
	}
}