using System.IO;
using HttpServer.FormDecoders;
using NUnit.Framework;
using HttpServer;
using HttpServer.FormDecoders;

namespace HttpServer.Test.FormDecoders
{
    [TestFixture]
    public class DecoderProviderTest
    {
        private FormDecoderProvider _provider;
        private Stream _stream;
        private MyDefaultDecoder _myDecoder;

        [SetUp]
        public void Setup()
        {
            _myDecoder = new MyDefaultDecoder();
            _provider = new FormDecoderProvider();
            _provider.Add(_myDecoder);
            _provider.Add(new XmlDecoder());
            _stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(_stream);
            writer.WriteLine("<user><firstname>jonas</firstname></user>");
            writer.Flush();
            _stream.Seek(0, SeekOrigin.Begin);
        }

        [Test]
        public void Test()
        {
            HttpInput input = _provider.Decode("text/xml", _stream, null);
            Assert.AreEqual("jonas", input["user"]["firstname"].Value);
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void TestExceptions1()
        {
            _provider.Decode(null, null, null);
        }

        [Test]
        public void TestDefaultDecoder()
        {
            _provider.DefaultDecoder = _myDecoder;
            _provider.Decode(null, _stream, null);
            Assert.IsTrue(_myDecoder.Called);
        }

        
    }
}
