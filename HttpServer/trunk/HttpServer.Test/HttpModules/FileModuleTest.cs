using System;
using System.IO;
using HttpServer.HttpModules;
using HttpServer.Test.TestHelpers;
using NUnit.Framework;
using HttpServer.Sessions;
using HttpServer.Exceptions;

namespace HttpServer.Test.HttpModules
{
    [TestFixture]
    public class FileModuleTest
    {
        private IHttpRequest _request;
        private IHttpResponse _response;
        private HttpResponseContext _context;
        private FileModule _module;

        [SetUp]
        public void Setup()
        {
            _request = new HttpTestRequest();
            _request.HttpVersion = "HTTP/1.1";
            _context = new HttpResponseContext();
            _response = new HttpResponse(_context, _request);
            _module = new FileModule("/files/", Environment.CurrentDirectory);
            _module.MimeTypes.Add("txt", "text/plain");
        }


        [Test]
        public void TestTextFile()
        {
            //MyStream is not working.
            _request.Uri = new Uri("http://localhost/files/HttpModules/TextFile1.txt");
            _module.Process(_request, _response, new MemorySession("name"));

            _context.Stream.Seek(0, SeekOrigin.Begin);
            TextReader reader = new StreamReader(_context.Stream);
            string text = reader.ReadToEnd();

            int pos = text.IndexOf("\r\n\r\n");
            Assert.IsTrue(pos >= 0);

            text = text.Substring(pos + 4);
            Assert.AreEqual("Hello World!", text);
        }

        [Test]
        [ExpectedException(typeof(ForbiddenException))]
        public void TestForbiddenExtension()
        {
            _request.Uri = new Uri("http://localhost/files/HttpModules/Forbidden.xml");
            _module.Process(_request, _response, new MemorySession("name"));
        }

        [Test]
        public void TestNotFound()
        {
            _request.Uri = new Uri("http://localhost/files/notfound.txt");
            Assert.IsFalse(_module.Process(_request, _response, new MemorySession("name")));
        }

        [Test]
        public void TestNotFound2()
        {
            _request.Uri = new Uri("http://localhost/files/notfound.txt");
            Assert.IsFalse(_module.CanHandle(_request.Uri));
        }

        [Test]
        public void TestCanHandle()
        {
            _request.Uri = new Uri("http://localhost/files/HttpModules/Forbidden.xml");
            Assert.IsTrue(_module.CanHandle(_request.Uri));
        }

        //todo: Test security exceptions (filesystem security)
    }
}
