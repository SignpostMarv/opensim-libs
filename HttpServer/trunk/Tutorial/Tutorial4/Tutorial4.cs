using System.Net;
using HttpServer;
using HttpServer.Authentication;
using HttpServer.Exceptions;

namespace Tutorial.Tutorial4
{
    class Tutorial4 : Tutorial
    {
        private HttpServer.HttpServer _server;

        class User
        {
            public int id;
            public string userName;
            public User(int id, string userName)
            {
                this.id = id;
                this.userName = userName;
            }
        }

        #region Tutorial Members

        public void StartTutorial()
        {
            _server = new HttpServer.HttpServer();

            // Let's use Digest authentication which is superior to basic auth since it
            // never sends password in clear text.
            DigestAuthentication auth = new DigestAuthentication();
            _server.AuthenticationModules.Add(auth);

            // the OnAuthenticate method is used to provide the auth module with a password
            // since we need to encrypt it to be able to compare it with the encrypted password from the browser/client.
            auth.OnAuthenticate += OnAuthenticate;

            // The OnAuthenticationRequired method determines which pages we use authentication for.
            // in this case we got a /membersonly/ section that we protect.
            auth.OnAuthenticationRequired += OnAuthenticationRequired;

            // Let's reuse our module from previous tutorial to handle pages.
            _server.Add(new Tutorial3.MyModule());

            // and start the server.
            _server.Start(IPAddress.Any, 8080);
        }

        private bool OnAuthenticationRequired(HttpRequest request)
        {
            // only required authentication for "/membersonly"
            return request.Uri.AbsolutePath.StartsWith("/membersonly");
        }

        /// <summary>
        /// Delegate used to let authentication modules authenticate the username and password.
        /// </summary>
        /// <param name="realm">Realm that the user want to authenticate in</param>
        /// <param name="userName">Username specified by client</param>
        /// <param name="password">Password supplied by the delagete</param>
        /// <param name="login">object that will be stored in a session variable called <see cref="AuthModule.AuthenticationTag"/> if authentication was successful.</param>
        /// <exception cref="ForbiddenException">throw forbidden exception if too many attempts have been made.</exception>
        private void OnAuthenticate(string realm, string userName, ref string password, out object login)
        {
            // digest authentication encrypts password which means that
            // we need to provide the authenticator with a stored password.

            // you should really query a DB or something
            if (userName == "arne")
            {
                password = "morsOlle";

                // login can be fetched from HttpSession in all modules
                login = new User(1, "arne");
            }
            else
            {
                password = string.Empty;
                login = null;
            }
        }

        public void EndTutorial()
        {
            _server.Stop();
        }

        #endregion
    }
}
