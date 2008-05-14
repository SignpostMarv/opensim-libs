using System;
using NUnit.Framework;

namespace HttpServer.Test
{
    [TestFixture]
    public class HttpCookieTest
    {

        [Test]
        public void Test()
        {
            DateTime expires = DateTime.Now;
            ResponseCookie cookie = new ResponseCookie("jonas", "mycontent", expires);
            Assert.AreEqual(expires, cookie.Expires);
            Assert.AreEqual("jonas", cookie.Name);
            Assert.AreEqual("mycontent", cookie.Value);
        }

        public void TestCookies()
        {
            RequestCookies cookies = new RequestCookies("name     =   value; name1=value1;\r\nname2\r\n=\r\nvalue2;name3=value3");
            Assert.AreEqual("value", cookies["name"].Value);
            Assert.AreEqual("value1", cookies["name1"].Value);
            Assert.AreEqual("value2", cookies["name2"].Value);
            Assert.AreEqual("value3", cookies["name3"].Value);
            Assert.IsNull(cookies["notfound"]);
            cookies.Clear();
            Assert.AreEqual(0, cookies.Count);
        }

        public void TestNullCookies()
        {
            RequestCookies cookies = new RequestCookies(null);
            Assert.AreEqual(0, cookies.Count);
            cookies = new RequestCookies(string.Empty);
            Assert.AreEqual(0, cookies.Count);
        }
        public void TestEmptyCookies()
        {
            ResponseCookies cookies = new ResponseCookies();
            Assert.AreEqual(0, cookies.Count);

            DateTime expires = DateTime.Now.AddDays(1);
            cookies.Add(new ResponseCookie("myname", "myvalue", expires));
            Assert.AreEqual(1, cookies.Count);
            Assert.AreEqual("myvalue", cookies["myname"].Value);
            Assert.AreEqual(expires, cookies["myname"].Expires);
        }
    }
}
