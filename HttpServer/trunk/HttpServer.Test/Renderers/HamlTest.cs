using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HttpServer.Rendering;
using NUnit.Framework;
using HttpServer.Rendering.Haml;

namespace HttpServer.Test.Renderers
{
    [TestFixture]
    public class HamlTest
    {
        private readonly TemplateManager _mgr = new TemplateManager();
        private readonly HamlGenerator _generator = new HamlGenerator();

        public HamlTest()
        {
            _mgr.Add("haml", _generator);
        }


        public void Test1()
        {
            Assert.AreEqual("<head><title>MyTitle</title></head>", _mgr.Render("hamlsamples/test1.haml"));
        }

        public void Test2()
        {
            string realText =
                @"<html>
	<head>
		<title>This is my superfine title</title>
		<script>alert(""Welcome to my world"");</script>
	</head>
</html>
";
            string text = _mgr.Render("hamlsamples/test2.haml");
            Assert.AreEqual(realText, text);
        }

        public void Test4()
        {
            StringReader reader = new StringReader(@"%script{type=""text/javascript""}
	function selectAll(source, cat) {
		var elems = $('tbl').getElementsByClassName(cat);
		for each (var item in elems)
			$(item).checked = source.checked;
	}");
            _generator.Parse(reader);
            _generator.PrintDocument();

            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            _generator.GenerateCode(writer);

        }
        public void Test3()
        {
            string text = _mgr.Render("hamlsamples/test3.haml", "a", 1);
        }

        public void TestLayout()
        {
            string text = _mgr.Render("hamlsamples/testlayout.haml", "data", "shit");
        }

        public void TestCodeTags()
        {
            string text = _mgr.Render("hamlsamples/codesample.haml", "i", 1);
        }
    }
}
