using System;
using HttpServer.HttpModules;
using HttpServer.Test.TestHelpers;
using NUnit.Framework;
using HttpServer.Sessions;

namespace HttpServer.Test.HttpModules
{
    [TestFixture]
    public class ControllerModuleTest
    {
        private TestController _controller;
        private IHttpRequest _request;
        private IHttpResponse _response;
        private IHttpClientContext _context;
        private ControllerModule _module;

        [SetUp]
        public void Setup()
        {
            _controller = new TestController();
            _request = new HttpTestRequest();
            _request.HttpVersion = "HTTP/1.1";
            _context = new HttpResponseContext();
            _response = new HttpResponse(_context, _request);
            _module = new ControllerModule();
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
