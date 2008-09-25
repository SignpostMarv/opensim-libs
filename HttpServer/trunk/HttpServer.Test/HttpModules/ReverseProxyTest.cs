using System;
using HttpServer.HttpModules;
using HttpServer.Test.Controllers;
using HttpServer.Test.TestHelpers;
using NUnit.Framework;

namespace HttpServer.Test.HttpModules
{
    /// <summary>
    /// A bit complicated test.
    /// We need to setup another webserver to be able to serve the proxy requests.
    /// </summary>
    [TestFixture]
    public class ReverseProxyTest
    {
        private IHttpRequest _request;
        private IHttpResponse _response;
        private IHttpClientContext _context;
        private MyStream _stream;
        private ReverseProxyModule _module;
        private HttpServer _server;

        [SetUp]
        public void Setup()
        {
            _request = new HttpTestRequest();
            _request.HttpVersion = "HTTP/1.1";
            _stream = new MyStream();
            _context = new HttpResponseContext();
            _response = new HttpResponse(_context, _request);
            _module = new ReverseProxyModule("http://localhost/", "http://localhost:4210/");
            _server = new HttpServer();
            
        }

        private void OnRequest(IHttpClientContext client, IHttpRequest request)
        {
            throw new NotImplementedException();
        }

    }
}
