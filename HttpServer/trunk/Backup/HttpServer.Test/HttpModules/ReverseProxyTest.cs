using System;
using HttpServer.HttpModules;
using HttpServer.Test.Controllers;
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
        private HttpRequest _request;
        private HttpResponse _response;
        private HttpClientContext _context;
        private MyStream _stream;
        private ReverseProxyModule _module;
        private HttpServer _server;

        [SetUp]
        public void Setup()
        {
            _request = new HttpRequest();
            _request.HttpVersion = "HTTP/1.1";
            _stream = new MyStream();
            _context = new HttpClientContext(false, _stream, OnRequest);
            _response = new HttpResponse(_context, _request);
            _module = new ReverseProxyModule("http://localhost/", "http://localhost:4210/");
            _server = new HttpServer();
            
        }

        private void OnRequest(HttpClientContext client, HttpRequest request)
        {
            throw new NotImplementedException();
        }

    }
}
