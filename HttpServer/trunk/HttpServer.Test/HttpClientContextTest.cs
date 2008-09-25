using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Fadd;
using NUnit.Framework;

namespace HttpServer.Test
{
    /// <summary>
    /// todo: either fix mystream or create a network stream and stuff to it through another socket.
    /// </summary>
    [TestFixture]
    public class HttpClientContextTest
    {
        private IHttpClientContext _context;
        private IHttpRequest _request;
        private ManualResetEvent _event = new ManualResetEvent(false);
        private ManualResetEvent _disconnectEvent = new ManualResetEvent(false);
        private bool _disconnected;
        private Socket _remoteSocket;
        private TcpClient _client;
        private Socket _listenSocket;
        private int _counter;

        public HttpClientContextTest()
        {
            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(new IPEndPoint(IPAddress.Any, 14862));
            _listenSocket.Listen(0);
        }

        [SetUp]
        public void Setup()
        {
            _disconnectEvent.Reset();
            _event.Reset();

            _counter = 0;
            IAsyncResult res = _listenSocket.BeginAccept(null, null);
            _client = new TcpClient();
            _client.Connect("localhost", 14862);
            _remoteSocket = _listenSocket.EndAccept(res);

            _context = new HttpClientContextImp(false, new IPEndPoint(IPAddress.Loopback, 21111), OnRequest, OnDisconnect, _client.GetStream(), ConsoleLogWriter.Instance);

            _request = null;
            _disconnected = false;
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _client.Close();
                 _remoteSocket.Close();
            }
            catch (SocketException)
            {
            }
        }

        [Test]
        [ExpectedException(typeof(CheckException))]
        public void TestConstructor1()
        {
            new HttpClientContextImp(true, new IPEndPoint(IPAddress.Loopback, 21111), null, null);
        }
        [Test]
        [ExpectedException(typeof(CheckException))]
        public void TestConstructor2()
        {
            new HttpClientContextImp(true, new IPEndPoint(IPAddress.Loopback, 21111), new MemoryStream(), null);
        }

        [Test]
        [ExpectedException(typeof(CheckException))]
        public void TestConstructor3()
        {
            MemoryStream stream = new MemoryStream();
            stream.Close();
            new HttpClientContextImp(true, new IPEndPoint(IPAddress.Loopback, 21111), stream, null);
        }

        [Test]
        [ExpectedException(typeof(CheckException))]
        public void TestConstructor4()
        {
            new HttpClientContextImp(true, new IPEndPoint(IPAddress.Loopback, 21111), null, null, null, null);
        }

        [Test]
        [ExpectedException(typeof(CheckException))]
        public void TestSecured()
        {
            MemoryStream stream = new MemoryStream();
            IHttpClientContext context = new HttpClientContextImp(true, new IPEndPoint(IPAddress.Loopback, 21111), stream, null);
            Assert.IsTrue(context.Secured);
        }

        [Test]
        [ExpectedException(typeof(CheckException))]
        public void TestSecured2()
        {
            MemoryStream stream = new MemoryStream();
            IHttpClientContext context = new HttpClientContextImp(false, new IPEndPoint(IPAddress.Loopback, 21111), stream, null);
            Assert.IsFalse(context.Secured);
        }

        private void OnDisconnect(IHttpClientContext client, SocketError error)
        {
            _disconnected = true;
            _disconnectEvent.Set();
        }

        private void OnRequest(IHttpClientContext client, IHttpRequest request)
        {
            ++_counter;
            _request = request;
            _event.Set();
        }

        [Test]
        public void TestRequest()
        {
            WriteStream("GET / HTTP/1.0\r\nhost: localhost\r\n\r\n");
            _event.WaitOne(5000, true);
            Assert.IsNotNull(_request);
            Assert.AreEqual("GET", _request.Method);
            Assert.AreEqual("/", _request.Uri.AbsolutePath);
            Assert.AreEqual(HttpHelper.HTTP10, _request.HttpVersion);
            Assert.AreEqual("localhost", _request.Headers["host"]);
        }

        [Test]
        public void TestValidInvalidValid()
        {
            WriteStream(@"GET / HTTP/1.0
host: localhost

someshot jsj

GET / HTTP/1.1
host:shit.se
accept:all");
            _disconnectEvent.WaitOne(5000, true);
            Assert.IsTrue(_disconnected);
        }

        [Test]
        public void TestTwoRequests()
        {
            WriteStream(@"GET / HTTP/1.0
host: localhost

GET / HTTP/1.1
host:shit.se
accept:all

");
            _event.WaitOne(500, true);
            _event.WaitOne(50, true);
            Assert.AreEqual(2, _counter);
            Assert.AreEqual("GET", _request.Method);
            Assert.AreEqual(HttpHelper.HTTP11, _request.HttpVersion);
            Assert.AreEqual("all", _request.Headers["accept"]);
            Assert.AreEqual("shit.se", _request.Headers["host"]);
        }

        [Test]
        public void TestPartial1()
        {
            WriteStream("GET / HTTP/1.0\r\nhost:");
            WriteStream("myhost");
            WriteStream("\r\n");
            WriteStream("accept:    all");
            WriteStream("\r\n");
            WriteStream("\r\n");
            _event.WaitOne(50000, true);
            Assert.IsNotNull(_request);
            Assert.AreEqual("GET", _request.Method);
            Assert.AreEqual(HttpHelper.HTTP10, _request.HttpVersion);
            Assert.AreEqual("all", _request.Headers["accept"]);
            Assert.AreEqual("myhost", _request.Headers["host"]);
        }

        [Test]
        public void TestPartials()
        {
            WriteStream("GET / ");
            WriteStream("HTTP/1.0\r\n");
            WriteStream("host:localhost\r\n");
            WriteStream("\r\n");
            _event.WaitOne(500, true);
            Assert.IsNotNull(_request);
            Assert.AreEqual("GET", _request.Method);
            Assert.AreEqual("HTTP/1.0", _request.HttpVersion);
            Assert.AreEqual("/", _request.Uri.AbsolutePath);
            Assert.AreEqual("localhost", _request.Headers["host"]);
        }

        [Test]
        public void TestPartialWithBody()
        {
            WriteStream("GET / ");
            WriteStream("HTTP/1.0\r\n");
            WriteStream("host:localhost\r\n");
            WriteStream("cOnTenT-LENGTH:11\r\n");
            WriteStream("Content-Type: text/plain");
            WriteStream("\r\n");
            WriteStream("\r\n");
            WriteStream("Hello");
            WriteStream(" World");
            _event.WaitOne(5000, false);
            Assert.IsNotNull(_request);
            Assert.AreEqual("GET", _request.Method);
            Assert.AreEqual("HTTP/1.0", _request.HttpVersion);
            Assert.AreEqual("/", _request.Uri.AbsolutePath);
            Assert.AreEqual("localhost", _request.Headers["host"]);

            StreamReader reader = new StreamReader(_request.Body);
            Assert.AreEqual("Hello World", reader.ReadToEnd());
        }

        private void WriteStream(string s)
        {
            _event.Reset();
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            _remoteSocket.Send(bytes);
            Thread.Sleep(50);
        }
    }
}
