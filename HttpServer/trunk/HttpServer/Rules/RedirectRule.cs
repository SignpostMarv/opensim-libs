using System;

namespace HttpServer.Rules
{
    /// <summary>
    /// redirects from one url to another.
    /// </summary>
    public class RedirectRule : IRule
    {
        private readonly string _fromUrl;
        private readonly string _toUrl;
        private readonly bool _shouldRedirect = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectRule"/> class.
        /// </summary>
        /// <param name="fromUrl">Absolute path (no servername)</param>
        /// <param name="toUrl">Absolute path (no servername)</param>
        /// <example>
        /// server.Add(new RedirectRule("/", "/user/index"));
        /// </example>
        public RedirectRule(string fromUrl, string toUrl)
        {
            _fromUrl = fromUrl;
            _toUrl = toUrl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectRule"/> class.
        /// </summary>
        /// <param name="fromUrl">Absolute path (no servername)</param>
        /// <param name="toUrl">Absolute path (no servername)</param>
        /// <param name="shouldRedirect">true if request should be redirected, false if the request uri should be replaced.</param>
        /// <example>
        /// server.Add(new RedirectRule("/", "/user/index"));
        /// </example>
        public RedirectRule(string fromUrl, string toUrl, bool shouldRedirect)
        {
            _fromUrl = fromUrl;
            _toUrl = toUrl;
            _shouldRedirect = shouldRedirect;
        }

        /// <summary>
        /// string to match request url with.
        /// </summary>
        /// <remarks>Is compared to request.Uri.AbsolutePath</remarks>
        public string FromUrl
        {
            get { return _fromUrl; }
        }

        /// <summary>
        /// Where to redirect.
        /// </summary>
        public string ToUrl
        {
            get { return _toUrl; }
        }

        /// <summary>
        /// true if we should redirect.
        /// </summary>
        /// <remarks>
        /// false means that the rule will replace
        /// the current request uri with the new one from this class.
        /// </remarks>
        public bool ShouldRedirect
        {
            get { return _shouldRedirect; }
        }

        /// <summary>
        /// Process the incoming request.
        /// </summary>
        /// <param name="request">incoming http request</param>
        /// <param name="response">outgoing http response</param>
        /// <returns>true if response should be sent to the browser directly (no other rules or modules will be processed).</returns>
        /// <remarks>
        /// returning true means that no modules will get the request. Returning true is typically being done
        /// for redirects.
        /// </remarks>
        public virtual bool Process(IHttpRequest request, IHttpResponse response)
        {
            if (request.Uri.AbsolutePath == FromUrl)
            {
                if (!ShouldRedirect)
                {
                	request.Uri = new Uri(request.Uri, ToUrl);
                	return false;
                }

            	response.Redirect(ToUrl);
                return true;
            }

            return false;
        }
    }
}