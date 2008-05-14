using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using HttpServer.Language;
using HttpServer.Language.Memory;
using NUnit.Framework;

namespace HttpServer.Test.Language
{
    [TestFixture]
    public class LanguageTest
    {
        private LanguageManager _langMgr;

    [SetUp]
        public void Setup()
        {
            _langMgr = new MemLanguageManager();
        }

        [Test]
        public void TestNoCategory()
        {
            Assert.IsNull(_langMgr.GetCategory("Invalid"));
        }

        [Test]
        public void TestEmpty()
        {
            Assert.IsNotNull(_langMgr.GetCategory(LanguageCategory.Default));
            Assert.AreEqual("[name]", _langMgr.GetCategory(LanguageCategory.Default)["name"]);
            Assert.AreEqual("[name]", _langMgr["name"]);
            Assert.AreEqual("[name]", _langMgr["name", "notExising"]);
            Assert.AreEqual("[name]", _langMgr["name", LanguageCategory.Default]);
        }

        [Test]
        public void TestOneLanguage()
        {
            _langMgr.Add("voodoo", 1053, "MyCat", "Voodoo är bra för magen.");
            Assert.IsNotNull(_langMgr.GetCategory("MyCat"));
            CultureInfo cultutre = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(1053);
            Assert.AreEqual("Voodoo är bra för magen.", _langMgr["voodoo"]);
            Assert.AreEqual("Voodoo är bra för magen.", _langMgr["voodoo", "MyCat"]);
            Assert.AreEqual("Voodoo är bra för magen.", _langMgr.GetCategory("MyCat")["voodoo"]);
            Assert.AreEqual("Voodoo är bra för magen.", _langMgr.GetCategory("MyCat")["voodoo", 1053]);
            Assert.AreEqual("[voodoo]", _langMgr.GetCategory("MyCat")["voodoo",1033]);
            Assert.AreEqual("[voodoo]", _langMgr["voodoo", "OtherCat"]);
            Assert.AreEqual("[voodoo]", _langMgr["voodoo", "MyCat", 1033]);

        }
    }
}
