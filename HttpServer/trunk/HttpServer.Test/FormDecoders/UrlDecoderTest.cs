using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using NUnit.Framework;
using HttpServer;
using HttpServer.FormDecoders;

namespace HttpServer.Test.FormDecoders
{
    [TestFixture]
    public class UrlDecoderTest
    {


        [Test]
        public void Test()
        {
            UrlDecoder decoder = new UrlDecoder();
            Assert.IsTrue(decoder.CanParse("application/x-www-form-urlencoded"));
            Assert.IsFalse(decoder.CanParse("text/plain"));
            Assert.IsFalse(decoder.CanParse(string.Empty));
            Assert.IsFalse(decoder.CanParse(null));

            MemoryStream stream = new MemoryStream();
            string urlencoded = HttpUtility.UrlEncode(@"user[firstname]=jonas&user[extension][id]=1&myname=jonas&user[firstname]=arne");
            byte[] bytes = Encoding.ASCII.GetBytes(urlencoded);
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            HttpInput input = decoder.Decode(stream, "application/x-www-form-urlencoded", Encoding.ASCII);
            Assert.AreEqual("jonas", input["myname"].Value);
            Assert.AreEqual(2, input["user"]["firstname"].Count);
            Assert.AreEqual("jonas", input["user"]["firstname"].Values[0]);
            Assert.AreEqual("arne", input["user"]["firstname"].Values[1]);
            Assert.AreEqual("1", input["user"]["extension"]["id"].Value);
            Assert.IsNull(input["unknow"].Value);
            Assert.AreEqual(HttpInputItem.Empty, input["unknown"]);
        }

        [Test]
        public void TestNull()
        {
            UrlDecoder decoder = new UrlDecoder();
            Assert.IsNull(decoder.Decode(new MemoryStream(), "application/x-www-form-urlencoded", Encoding.ASCII));
        }

        [Test]
        [ExpectedException("System.IO.InvalidDataException")]
        public void TestInvalidData()
        {
            UrlDecoder decoder = new UrlDecoder();
            MemoryStream stream = new MemoryStream();
            byte[] bytes = Encoding.ASCII.GetBytes(@"not encoded or anything");
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            Assert.IsNull(decoder.Decode(stream, "application/x-www-form-urlencoded", Encoding.ASCII));
        }

        [Test]
        public void TestNull2()
        {
            UrlDecoder decoder = new UrlDecoder();
            decoder.Decode(null, null, null);
        }
    }
}
