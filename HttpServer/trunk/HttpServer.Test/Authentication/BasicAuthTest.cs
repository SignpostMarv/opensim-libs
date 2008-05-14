using System;
using HttpServer.Authentication;
using NUnit.Framework;

namespace HttpServer.Test.Authentication
{
    [TestFixture]
    public class BasicAuthTest
    {
        private BasicAuthentication _auth;

        /* Wikipedia:
         * http://en.wikipedia.org/wiki/Basic_access_authentication
         * 
         * "Aladdin:open sesame"
         * should equal
         * "QWxhZGRpbjpvcGVuIHNlc2FtZQ=="
         */

        [SetUp]
        public void Setup()
        {
            _auth = new BasicAuthentication();
            _auth.OnAuthenticate += OnAuth;
            _auth.OnAuthenticationRequired += OnRequired;
        }

        private void OnAuth(string realm, string userName, ref string password, out object login)
        {
            Assert.AreEqual("myrealm", realm, "Realms do not match");
            Assert.AreEqual("Aladdin", userName, "Username do not match");

            password = "open sesame";
            login = "mylogin";
        }

        private bool OnRequired(HttpRequest request)
        {
            return true;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestResponse1()
        {
            _auth.CreateResponse(null, false);
        }

        [Test]
        public void TestResponse2()
        {
            string response = _auth.CreateResponse("myrealm", false);
            Assert.AreEqual("Basic realm=\"myrealm\"", response);
        }

        [Test]
        public void TestAuth()
        {
            _auth.Authenticate("Basic " + "QWxhZGRpbjpvcGVuIHNlc2FtZQ==", "myrealm", "POST", false);
            //OnAuth will to the checks
        }

    }
}
