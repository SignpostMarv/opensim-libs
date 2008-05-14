using System.IO;
using System.Text;
using HttpServer.Rendering;
using NUnit.Framework;
using TinyGenerator=HttpServer.Rendering.Tiny.TinyGenerator;

namespace HttpServer.Test.Renderers
{
    [TestFixture]
    public     class TinyTest
    {
        private TinyGenerator _gen = new TinyGenerator();
        private StringBuilder _sb;
        TemplateCompiler _compiler = new TemplateCompiler(); 

        public TinyTest()
        {
        }

        [SetUp]
        public void Setup()
        {
            _sb  = new StringBuilder();
        }

        [Test]
        public void TestEcho()
        {
            string temp = @"<html name=""<%= name %>"">";
            ParseAndGenerate(temp);
            Assert.AreEqual("sb.Append(@\"<html name=\"\"\");sb.Append( name );sb.Append(@\"\"\">\r\n\");", _sb.ToString());
        }

        [Test]
        public void TestCode()
        {
            string temp = @"<html><% if (a == 'a') { %>Hello<% } %></html>";
            ParseAndGenerate(temp);
            Assert.AreEqual("sb.Append(@\"<html>\"); if (a == 'a') { sb.Append(@\"Hello\"); } sb.Append(@\"</html>\r\n\");", _sb.ToString());
        }

        public void TestEchoGenerated()
        {
            ParseAndGenerate(@"<html name=""<%= name %>"">");
            object[] args = new object[]{"name", "jonas"};
            TinyTemplate template = _compiler.Compile(args, _sb.ToString());
            string result = template.Invoke(args);
            Assert.AreEqual("<html name=\"jonas\">\r\n", result);
        }

        public void TestCodeGenerated()
        {
            ParseAndGenerate(@"<html><% if (a == 'a') { %>Hello<% } %></html>");
            object[] args = new object[] { 'a', 'b' };
            TinyTemplate template = _compiler.Compile(args, _sb.ToString());

            Assert.AreEqual("<html></html>\r\n", template.Invoke(args));
            Assert.AreEqual("<html>Hello</html>\r\n", template.Invoke(new object[] { 'a', 'a' }));
        }

        private void ParseAndGenerate(string temp)
        {
            StringReader reader = new StringReader(temp);
            _gen.Parse(reader);

            StringWriter writer = new StringWriter(_sb);
            _gen.GenerateCode(writer);
        }
    }
}
