using System.IO;
using System.Text;
using HttpServer.Rendering;
using HttpServer.Rendering.Haml;
using HttpServer.Rendering.Haml.Nodes;
using NUnit.Framework;
using HttpServer.Exceptions;

namespace HttpServer.Test.Renderers
{
	[TestFixture]
	public class PartialNodeTest
	{
		[Test]
		public void TestParamsSimple()
		{
			string output = Parse("_\"/user/new/\"{parameterName=\"parameterValue\",parameterValue2=parameter}");

			Assert.AreEqual("sb.Append(@\"\");sb.Append(hiddenTemplateManager.RenderPartial(\"user\\\\new.haml\", new TemplateArguments(\"parameterName\", \"parameterValue\", \"parameterValue2\", parameter), args));", output);
		}

		[Test]
		public void TestParamsAdvanced()
		{
			string output = Parse("_\"/test/\"{user=CurrentUser:typeof(User)}");

			Assert.AreEqual("sb.Append(@\"\");sb.Append(hiddenTemplateManager.RenderPartial(\"test.haml\", new TemplateArguments(\"user\", CurrentUser, typeof(User)), args));", output);
		}

		[Test]
		[ExpectedException(typeof(CodeGeneratorException))]
		public void TestInvalidParanthesis()
		{
			Parse("_\"/test/");
		}

		[Test]
		[ExpectedException(typeof(CodeGeneratorException))]
		public void TestInvalidModifier()
		{
			Parse("_\"/test/\"{test=\"test\"}{class=\"test\"}");
		}

		protected static string Parse(string text)
		{
			HamlGenerator parser = new HamlGenerator();
			TextReader input = new StringReader(text);
			parser.Parse(input);

			StringBuilder sb = new StringBuilder();
			TextWriter output = new StringWriter(sb);
			parser.GenerateCode(output);

			return output.ToString();
		}
	}
}
