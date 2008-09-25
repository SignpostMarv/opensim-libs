using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using HttpServer.Rendering.Haml.Nodes;
using HttpServer.Rendering.Haml;

namespace HttpServer.Test.Renderers
{
	[TestFixture]
	public class TagNodeTest
	{
		[Test]
		public void TestSelfClosing()
		{
			HamlGenerator parser = new HamlGenerator();

			TextReader input = new StringReader("%img{src=\"bild\",border=\"1\"}");
			parser.Parse(input);

			StringWriter output = new StringWriter(new StringBuilder());
			parser.GenerateCode(output);

			Assert.AreEqual("sb.Append(@\"<img src=\"\"bild\"\" border=\"\"1\"\"/>\");", output.ToString());
		}
	}
}
