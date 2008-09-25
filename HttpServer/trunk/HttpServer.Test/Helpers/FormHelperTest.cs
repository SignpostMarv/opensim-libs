using HttpServer.Helpers;
using NUnit.Framework;

namespace HttpServer.Test.Helpers
{/* todo: fix these tests
    [TestFixture]
    public class FormHelperTest
    {
        [Test]
        public void TestAjax()
        {
            string start = FormHelper.Start("myName", "/user/new/", true);
            Assert.AreEqual(@"<form name=""myName"" action=""/user/new/"" method=""post"" id=""myName"" onsubmit="""
                            + FormHelper.JSImplementation.AjaxFormOnSubmit() + "return false;\">", start);
        }

        [Test]
        public void TestAjaxOnSubmit()
        {
            string start = FormHelper.Start("myName", "/user/new/", true, "onsubmit", "alert('hello world')");
            Assert.AreEqual(
                @"<form name=""myName"" action=""/user/new/"" id=""myName"" method=""post"" onsubmit=""alert('hello world');"
                + FormHelper.JSImplementation.AjaxFormOnSubmit() + "return false;\">", start);
        }

        [Test]
        public void TestAjaxOnSubmit2()
        {
            string start = FormHelper.Start("myName", "/user/new/", true, "onsuccess:", "alert('Hello world!')");
            Assert.AreEqual(@"<form name=""myName"" action=""/user/new/"" id=""myName"" method=""post"" onsubmit="""
                            + FormHelper.JSImplementation.AjaxFormOnSubmit("onsuccess:", "alert(\'Hello world!\')") +
                            "return false;\">", start);
        }

        [Test]
        public void TestAjaxOnSubmitAndDelete()
        {
            string start = FormHelper.Start("myName", "/user/new/", true, "onsubmit", "alert('hello world')", "method",
                                            "delete");
            Assert.AreEqual(
                @"<form name=""myName"" action=""/user/new/"" id=""myName"" onsubmit=""alert(\'hello world\');"
                + FormHelper.JSImplementation.AjaxFormOnSubmit() + @"return false;"" method=""delete"">", start);
        }

        [Test]
        public void TestExtraAttributes()
        {
            string start = FormHelper.Start("myName", "/user/new/", false, "class", "worldClass", "style",
                                            "display:everything;");
            Assert.AreEqual(
                @"<form name=""myName"" action=""/user/new/"" id=""myName"" method=""post"" class=""worldClass"" style=""display:everything;"">",
                start);
        }

        [Test]
        public void TestStart()
        {
            string start = FormHelper.Start("myName", "/user/new/", false);
            Assert.AreEqual(@"<form name=""myName"" action=""/user/new/"" id=""myName"" method=""post"">", start);
        }

        [Test]
        public void TestStartDelete()
        {
            string start = FormHelper.Start("myName", "/user/new/", false, "method", "delete");
            Assert.AreEqual(@"<form name=""myName"" action=""/user/new/"" id=""myName"" method=""delete"">", start);
        }
    }
  * */
}