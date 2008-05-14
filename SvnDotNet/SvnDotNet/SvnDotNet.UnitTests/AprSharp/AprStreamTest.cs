using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.UnitTests.AprSharp {
    [TestFixture]
    public class AprStreamTest {
        AprPool _pool;

        [TestFixtureSetUp]
        public void Init()
        {
            _pool = AprPool.Create();
        }

        [TestFixtureTearDown]
        public void Destroy()
        {
            _pool.Destroy();
        }

        [Test]
        public void CreateDestroy()
        {
            AprStream s = new AprStream(_pool);
            Assert.IsNotNull(s, "#A01");

            Assert.IsFalse(s.AprFileInput.IsNull, "#A02");
            Assert.IsFalse(s.AprFileOutput.IsNull, "#A03");
            s.Close();
        }

        [Test]
        public void InputOutput()
        {
            AprStream s = new AprStream(_pool);
            byte[] fooBytes = { 0x66, 0x6f, 0x6f, 0x0d, 0x0a };

            s.Write(fooBytes, 0, 5);
            s.AprFileInput.Close();

            StreamReader reader = new StreamReader(s);
            string fooString = reader.ReadToEnd();
            Assert.AreEqual(fooString, "foo\r\n", "#B01");
        }
    }
}
