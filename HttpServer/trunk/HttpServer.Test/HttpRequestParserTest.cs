using System;
using System.Text;
using HttpServer.Exceptions;
using NUnit.Framework;

namespace HttpServer.Test
{
    [TestFixture]
    public class HttpRequestParserTest
    {
        private HttpRequestParser _parser;
        private IHttpRequest _request;

        [SetUp]
        public void Setup()
        {
            _parser = new HttpRequestParser(OnRequestCompleted, ConsoleLogWriter.Instance);
            _request = null;
        }

        [Test]
        public void TestRequestLine()
        {
            Parse("GET / HTTP/1.0\r\n\r\n");
            Assert.IsNotNull(_request);
            Assert.AreEqual("GET", _request.Method);
            Assert.AreEqual("HTTP/1.0", _request.HttpVersion);
            Assert.AreEqual("/", _request.Uri.AbsolutePath);
            Assert.IsNull(_request.Headers["host"]);
        }

        [Test]
        public void TestSimpleHeader()
        {
            Parse(@"GET / HTTP/1.0
host: www.gauffin.com
accept: text/html

");
            Assert.IsNotNull(_request);
            Assert.AreEqual("GET", _request.Method);
            Assert.AreEqual("HTTP/1.0", _request.HttpVersion);
            Assert.AreEqual("/", _request.Uri.AbsolutePath);
            Assert.AreEqual("www.gauffin.com", _request.Headers["host"]);
            Assert.AreEqual("text/html", _request.Headers["accept"]);
        }


        [Test]
        [ExpectedException(typeof(BadRequestException))]
        public void TestInvalidFirstLine()
        {
            Parse("GET HTTP/1.0 /\r\n\r\n");
        }

        [Test]
        [ExpectedException(typeof(BadRequestException))]
        public void TestJunkRequestLine()
        {
            StringBuilder sb = new StringBuilder();
            Random r = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < 10000; ++i)
                sb.Append(r.Next(1, 254));

            Parse(sb.ToString());
        }

        [Test]
        [ExpectedException(typeof(BadRequestException))]
        public void TestJunkRequestLine2()
        {
            Parse("\r\n\r\n");
        }

        [Test]
        [ExpectedException(typeof(BadRequestException))]
        public void TestTooLargeHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("GET / HTTP/1.0\r\n");
            Random r = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < 300; ++i)
                sb.Append(r.Next('A', 'Z'));
            sb.Append(": ");
            for (int i = 0; i < 4000; ++i)
                sb.Append(r.Next('A', 'Z'));
            sb.Append("\r\n\r\n");
            Parse(sb.ToString());
        }

        [Test]
        [ExpectedException(typeof(BadRequestException))]
        public void TestTooLargeHeader2()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("GET / HTTP/1.0\r\n");
            Random r = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < 40; ++i)
                sb.Append(r.Next('A', 'Z'));
            sb.Append(": ");
            for (int i = 0; i < 8000; ++i)
                sb.Append(r.Next('A', 'Z'));
            sb.Append("\r\n\r\n");
            Parse(sb.ToString());
        }

        [Test]
        public void TestSameHeader()
        {
            Parse("GET / HTTP/1.0\r\nmyh: test\r\nmyh: hello\r\n\r\n");
            Assert.AreEqual("test,hello", _request.Headers["myh"]);
        }

        [Test]
        [ExpectedException(typeof(BadRequestException))]
        public void TestCorrupHeader()
        {
            Parse("GET / HTTP/1.0\r\n: test\r\n\r\n");
        }

/*        [Test]
        [ExpectedException(typeof(BadRequestException))]
        public void TestEmptyHeader()
        {
            Parse("GET / HTTP/1.0\r\nname:\r\n\r\n");
        }
        */
        public void TestMultipleLines()
        {
            Parse("GET / HTTP/1.0\r\nHost:\r\n  hello\r\n\r\n");
            Assert.AreEqual("hello", _request.Headers["host"]);
        }

        public void TestWhiteSpaces()
        {
            Parse("GET / HTTP/1.0\r\nHost        :           \r\n    hello\r\n\r\n");
            Assert.AreEqual("hello", _request.Headers["host"]);
        }

        public void TestSpannedHeader()
        {
            Parse("GET / HTTP/1.0\r\nmyheader: my long \r\n name of header\r\n\r\n");
            Assert.AreEqual("my long name of header", _request.Headers["myheader"]);
        }

        public void TestBlockParse()
        {
            Parse("GET / HTTP/1.0\r\n");
            Parse("host: myname\r\n");
            Parse("myvalue:");
            Parse("nextheader\r\n");
            Parse("\r\n");
            Assert.AreEqual("myname", _request.Headers["host"]);
            Assert.AreEqual("nextheader", _request.Headers["myvalue"]);
        }

        [Test]
        public void TestMultipleRequests()
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes("GET / HTTP/1.0\r\nhost:bahs\r\n\r\nGET / HTTP/1.0\r\nusername:password\r\n\r\n");
            int bytesHandled = _parser.ParseMessage(bytes, 0, bytes.Length);
            Assert.AreEqual("bahs", _request.Headers["host"]);
            Assert.AreEqual(29, bytesHandled);
            byte[] buffer2 = new byte[40];
            Array.Copy(bytes, bytesHandled, buffer2, 0, bytes.Length - bytesHandled);
            Assert.AreEqual(37, _parser.ParseMessage(buffer2, 0, bytes.Length - bytesHandled));
            Assert.AreEqual("password", _request.Headers["username"]);
        }

        [Test]
        [ExpectedException(typeof(BadRequestException))]
        public void TestCorrectRequest_InvalidRequest()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(@"GET / HTTP/1.0
host:bahs

GET incorrect HTP/11

GET / HTTP/1.0
username:password

");
            int bytesLeft = _parser.ParseMessage(bytes, 0, bytes.Length);
            Assert.AreEqual("bahs", _request.Headers["host"]);
            byte[] buffer2 = new byte[100];
            Array.Copy(bytes, bytes.Length - bytesLeft, buffer2, 0, bytesLeft);
            Assert.AreEqual(0, _parser.ParseMessage(buffer2, 0, bytesLeft));
            Assert.AreEqual("password", _request.Headers["username"]);
        }

        [Test]
        [ExpectedException(typeof(BadRequestException))]
        public void TestTwoRequests()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(@"GET / HTTP/1.0
host:bahs

GET / HTTP/1.1
host: mah

GET / HTTP/1.0
username:password

");
            int bytesLeft = _parser.ParseMessage(bytes, 0, bytes.Length);
            Assert.AreEqual("bahs", _request.Headers["host"]);
            byte[] buffer2 = new byte[100];
            Array.Copy(bytes, bytes.Length - bytesLeft, buffer2, 0, bytesLeft);
            Assert.AreEqual(0, _parser.ParseMessage(buffer2, 0, bytesLeft));
            Assert.AreEqual("password", _request.Headers["username"]);
        }

        public void TestFormPost()
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    @"GET /user/dologin HTTP/1.1
Host: localhost:8081
User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; sv-SE; rv: 1.8.1.13) Gecko/20080311 Firefox/2.0.0.13
Accept: text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5
Accept-Language: sv,en-us;q=0.7,en;q=0.3
Accept-Encoding: gzip,deflate
Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7
Keep-Alive: 300
Connection: keep-alive
Referer: http: //localhost:8081/user/login
Cookie: style=Trend
Content-Type: application/x-www-form-urlencoded
Content-Length: 35

username=jonas&password=krakelkraka");
            int bytesHandled = _parser.ParseMessage(bytes, 0, bytes.Length);
            Assert.AreEqual(bytes.Length, bytesHandled);
            Assert.AreEqual(35, _request.Body.Length);
        }
        public int Parse(string s)
        {
            byte[] buffer = GetBytes(s);
            return _parser.ParseMessage(buffer, 0, buffer.Length);
        }

        public byte[] GetBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
        private void OnRequestCompleted(IHttpRequest request)
        {
            _request = request;
        }
    }
}
