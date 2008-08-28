using System;
using System.IO;
using NUnit.Framework;
using HttpServer.Sessions;

namespace HttpServer.Test.Controllers
{
    [TestFixture]
    class RequestControllerTest
    {
        private MyController _controller;
        private HttpRequest _request;
        private HttpResponse _response;
        private HttpClientContext _context;
        private MyStream _stream;

        [SetUp]
        public void Setup()
        {
            _controller = new MyController();
            _request = new HttpRequest();
            _request.HttpVersion = "HTTP/1.1";
            _stream = new MyStream();
            _context = new HttpClientContext(false, _stream, OnRequest);
            _response = new HttpResponse(_context, _request);
        }

        private void OnRequest(HttpClientContext client, HttpRequest request)
        {
            
        }

        [Test]
        public void TestTextMethod()
        {
            _request.Uri = new Uri("http://localhost/my/helloworld");
            Assert.IsTrue(_controller.Process(_request, _response, new MemorySession("myid")));

            _response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(_response.Body);
            Assert.AreEqual("HelloWorld", reader.ReadToEnd());
        }

        [Test]
        public void TestBinaryMethod()
        {
            _request.Uri = new Uri("http://localhost/my/raw");
            _response.Connection = ConnectionType.KeepAlive;
            Assert.IsTrue(_controller.Process(_request, _response, new MemorySession("myid")));

            _stream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(_stream);
            string httpResponse = reader.ReadToEnd();
            Assert.IsNotNull(httpResponse);

            int pos = httpResponse.IndexOf("\r\n\r\n");
            Assert.IsTrue(pos >= 0);

            httpResponse = httpResponse.Substring(pos + 4);
            Assert.AreEqual("Hello World", httpResponse);
            _stream.Signal();
        }

        [Test]
        public void TestUnknownMethod()
        {
            _request.Uri = new Uri("http://localhost/wasted/beer");
            _response.Connection = ConnectionType.KeepAlive;
            Assert.IsFalse(_controller.Process(_request, _response, new MemorySession("myid")));
        }


    }
}