using HttpServer;

namespace HttpServer.HttpModules
{
    /// <summary>
    /// A HttpModule can be used to serve urls. The module itself
    /// decides if it should serve a url or not. In this way, you can
    /// get a very flexible http app since you can let multiple modules
    /// serve almost similiar urls.
    /// </summary>
    /// <remarks>
    /// Throw UnauthorizedException if you are using a AuthenticationModule and want to prompt for username/password.
    /// </remarks>
    public abstract class HttpModule
    {
        /// <summary>
        /// Method that process the url
        /// </summary>
        /// <param name="request">Information sent by the browser about the request</param>
        /// <param name="response">Information that is being sent back to the client.</param>
        /// <param name="session">Session used to </param>
        /// <returns>true if this module handled the request.</returns>
        public abstract bool Process(HttpRequest request, HttpResponse response, HttpSession session);
        /*
        /// <summary>
        /// Checks if authentication is required by the module.
        /// </summary>
        /// <param name="request">Information sent by the browser about the request</param>
        /// <param name="response">Information that is being sent back to the client.</param>
        /// <param name="session">Session used to </param>
        /// <param name="cookies">Incoming/outgoing cookies. If you modify a cookie, make sure that you also set a expire date. Modified cookies will automatically be sent.</param>
        /// <returns>true authentication should be used.</returns>
        public abstract bool IsAuthenticationRequired(HttpRequest request, HttpResponse response, HttpSession session,
                                                      HttpCookies cookies);
         * */
    }
}
