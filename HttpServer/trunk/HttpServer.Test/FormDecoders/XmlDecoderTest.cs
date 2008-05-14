using System.IO;
using System.Text;
using NUnit.Framework;
using HttpServer;
using HttpServer.FormDecoders;

namespace HttpServer.Test.FormDecoders
{
    [TestFixture]
    public class XmlDecoderTest
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Test()
        {
            XmlDecoder decoder = new XmlDecoder();
            Assert.IsTrue(decoder.CanParse("text/xml"));
            Assert.IsFalse(decoder.CanParse("text/plain"));
            Assert.IsFalse(decoder.CanParse("xml"));
            Assert.IsFalse(decoder.CanParse("text"));
            Assert.IsFalse(decoder.CanParse(null));

            MemoryStream stream = new MemoryStream();
            byte[] bytes = Encoding.ASCII.GetBytes(@"<user lastname=""gauffin""><firstname>jonas</firstname></user>");
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            HttpInput input = decoder.Decode(stream, "text/xml", Encoding.ASCII);
            Assert.AreEqual("gauffin", input["user"]["lastname"].Value);
            Assert.AreEqual("jonas", input["user"]["firstname"].Value);
            Assert.IsNull(input["unknow"].Value);
            Assert.AreEqual(HttpInputItem.Empty, input["unknown"]);
        }

        [Test]
        public void TestNull()
        {
            XmlDecoder decoder = new XmlDecoder();
            Assert.IsNull(decoder.Decode(new MemoryStream(), "text/xml", Encoding.ASCII));
        }

        [Test]
        [ExpectedException("System.IO.InvalidDataException")]
        public void TestInvalidData()
        {
            XmlDecoder decoder = new XmlDecoder();
            MemoryStream stream = new MemoryStream();
            byte[] bytes = Encoding.ASCII.GetBytes(@"<user lastname=""gauffin""><firstname>jonas</firstname>");
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            Assert.IsNull(decoder.Decode(stream, "text/xml", Encoding.ASCII));
        }

        [Test]
        public void TestNull2()
        {
            XmlDecoder decoder = new XmlDecoder();
            decoder.Decode(null, null, null);
        }
    }
}
