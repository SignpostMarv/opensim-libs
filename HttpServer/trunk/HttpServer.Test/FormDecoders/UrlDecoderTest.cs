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
        public void TestLarge()
        {
            string url =
                "ivrMenu[Name]=Huvudmeny&ivrMenu[ExtensionId]=6&ivrMenu[OpenPhraseId]=267&ivrMenu[ScheduleId]=1&ivrMenu[ClosePhraseId]=268&ivrMenu[CloseActionId]=3&ivrMenu[CloseActionValue]=26&ivrMenu[TimeoutPhraseId]=267&ivrMenu[TimeoutActionId]=&ivrMenu[TimeoutActionValue]=&ivrMenu[TimeoutSeconds]=10&ivrMenu[Digits][1][Digit]=1&ivrMenu[Digits][1][ActionId]=1&ivrMenu[Digits][1][ActionValue]=49&ivrMenu[Digits][2][Digit]=2&ivrMenu[Digits][2][ActionId]=&ivrMenu[Digits][2][ActionValue]=&ivrMenu[Digits][3][Digit]=3&ivrMenu[Digits][3][ActionId]=&ivrMenu[Digits][3][ActionValue]=&ivrMenu[Digits][4][Digit]=4&ivrMenu[Digits][4][ActionId]=&ivrMenu[Digits][4][ActionValue]=&ivrMenu[Digits][5][Digit]=5&ivrMenu[Digits][5][ActionId]=&ivrMenu[Digits][5][ActionValue]=&ivrMenu[Digits][6][Digit]=6&ivrMenu[Digits][6][ActionId]=&ivrMenu[Digits][6][ActionValue]=&ivrMenu[Digits][7][Digit]=7&ivrMenu[Digits][7][ActionId]=&ivrMenu[Digits][7][ActionValue]=&ivrMenu[Digits][8][Digit]=8&ivrMenu[Digits][8][ActionId]=&ivrMenu[Digits][8][ActionValue]=&ivrMenu[Digits][9][Digit]=9&ivrMenu[Digits][9][ActionId]=&ivrMenu[Digits][9][ActionValue]=&ivrMenu[Digits][0][ActionId]=&ivrMenu[Digits][0][ActionValue]=&ivrMenu[Digits][*][ActionId]=&ivrMenu[Digits][*][ActionValue]=&ivrMenu[Digits][#][ActionId]=&ivrMenu[Digits][#][ActionValue]=";

            UrlDecoder decoder = new UrlDecoder();
            Assert.IsTrue(decoder.CanParse("application/x-www-form-urlencoded"));
            Assert.IsFalse(decoder.CanParse("text/plain"));
            Assert.IsFalse(decoder.CanParse(string.Empty));
            Assert.IsFalse(decoder.CanParse(null));

            MemoryStream stream = new MemoryStream();
            string urlencoded = HttpUtility.UrlEncode(url);
            byte[] bytes = Encoding.ASCII.GetBytes(urlencoded);
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            HttpInput input = decoder.Decode(stream, "application/x-www-form-urlencoded", Encoding.ASCII);
            Assert.AreEqual("Huvudmeny", input["ivrMenu"]["Name"].Value);
            Assert.AreEqual("6", input["ivrMenu"]["ExtensionId"].Value);
            Assert.AreEqual("267", input["ivrMenu"]["OpenPhraseId"].Value);
            Assert.AreEqual("1", input["ivrMenu"]["Digits"]["1"]["Digit"].Value);
            Assert.AreEqual("1", input["ivrMenu"]["Digits"]["1"]["ActionId"].Value);
            Assert.AreEqual("49", input["ivrMenu"]["Digits"]["1"]["ActionValue"].Value);
        }

        [Test]
        public void TestLogin()
        {
            string url =
                "email=somewhere%40gauffin.com&password=myPassWord";

            UrlDecoder decoder = new UrlDecoder();
            Assert.IsTrue(decoder.CanParse("application/x-www-form-urlencoded"));
            Assert.IsFalse(decoder.CanParse("text/plain"));
            Assert.IsFalse(decoder.CanParse(string.Empty));
            Assert.IsFalse(decoder.CanParse(null));

            MemoryStream stream = new MemoryStream();
            string urlencoded = url;
            byte[] bytes = Encoding.ASCII.GetBytes(urlencoded);
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            HttpInput input = decoder.Decode(stream, "application/x-www-form-urlencoded", Encoding.ASCII);
            Assert.AreEqual("somewhere@gauffin.com", input["email"].Value);
            Assert.AreEqual("myPassWord", input["password"].Value);
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
