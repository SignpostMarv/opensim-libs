using System;
using System.Collections.Generic;
using System.Text;
using HttpServer.Helpers;
using HttpServer.Helpers.Implementations;
using NUnit.Framework;

namespace HttpServer.Test.Helpers
{
    [TestFixture]
    public class PrototypeTest
    {
        PrototypeImp _imp = new PrototypeImp();

        [Test]
        public void TestOnSubmit()
        {
            string res = _imp.AjaxFormOnSubmit("onsuccess", "alert('Hello world!');");
            Assert.AreEqual("new Ajax.Request(this.action, { parameters: Form.serialize(this), method: 'post', asynchronous: true, evalScripts: true });", res);
            res = _imp.AjaxFormOnSubmit("onsuccess:", "alert('Hello world!');");
            Assert.AreEqual("new Ajax.Request(this.action, { parameters: Form.serialize(this), onsuccess: alert(\'Hello world!\');, method: 'post', asynchronous: true, evalScripts: true });", res);

            string ajax = JSHelper.AjaxUpdater("/test", "theField");
            res = _imp.AjaxFormOnSubmit("onsuccess:", ajax);
            Assert.AreEqual("new Ajax.Request(this.action, { parameters: Form.serialize(this), onsuccess: " + ajax + ", method: 'post', asynchronous: true, evalScripts: true });", res);
        }
    }
}
