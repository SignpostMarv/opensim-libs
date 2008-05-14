using System;
using System.Net;
using HttpServer.HttpModules;
using HttpServer.Test.Controllers;
using NUnit.Framework;
using HttpServer.Sessions;

namespace HttpServer.Test.HttpModules
{
    [TestFixture]
    public class ControllerModuleTest
    {
        private TestController _controller;
        private HttpRequest _request;
        private HttpResponse _response;
        private HttpClientContext _context;
        private MyStream _stream;
        private ControllerModule _module;

        [SetUp]
        public void Setup()
        {
            _controller = new TestController();
            _request = new HttpRequest();
            _request.HttpVersion = "HTTP/1.1";
            _stream = new MyStream();
            _context = new HttpClientContext(false, _stream, OnRequest);
            _response = new HttpResponse(_context, _request);
            _module = new ControllerModule();
        }

        private void OnRequest(HttpClientContext client, HttpRequest request)
        {
        }

        [Test]
        public void Test()
        {
            _module.Add(_controller);
            Assert.AreSame(_controller, _module["test"]);

            _request.Uri = new Uri("http://localhost/test/mytest/");
            _module.Process(_request, _response, new MemorySession("name"));
            Assert.AreEqual("MyTest", _controller.Method);

            _request.Uri = new Uri("http://localhost/test/myraw/");
            _module.Process(_request, _response, new MemorySession("name"));
            Assert.AreEqual("MyRaw", _controller.Method);
        }

        [Test]
        public void TestNoController()
        {
            _module.Add(_controller);
            _request.Uri = new Uri("http://localhost/test/nomethod/");
            Assert.IsFalse(_module.Process(_request, _response, new MemorySession("name")));
            Assert.AreEqual(null, _controller.Method);

            _request.Uri = new Uri("http://localhost/tedst/nomethod/");
            Assert.IsFalse(_module.Process(_request, _response, new MemorySession("name")));
            Assert.AreEqual(null, _controller.Method);
        }
    }
}
