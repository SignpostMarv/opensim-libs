using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using HttpServer.Exceptions;
using NUnit.Framework;

namespace HttpServer.Test.Exceptions
{
    [TestFixture]
    class HttpExceptionTest
    {
        [Test]
        public void Test()
        {
            HttpException ex = new HttpException(HttpStatusCode.Forbidden, "mymessage");
            Assert.AreEqual(HttpStatusCode.Forbidden, ex.HttpStatusCode);
            Assert.AreEqual("mymessage", ex.Message);
        }
    }
}
