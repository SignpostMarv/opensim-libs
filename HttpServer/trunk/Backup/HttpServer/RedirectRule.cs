namespace HttpServer
{
    /// <summary>
    /// redirects from one url to another.
    /// </summary>
    public class RedirectRule
    {
        private readonly string _fromUrl;
        private readonly string _toUrl;

        public RedirectRule(string fromUrl, string toUrl)
        {
            _fromUrl = fromUrl;
            _toUrl = toUrl;
        }

        public bool Process(HttpRequest request, HttpResponse response)
        {
            if (request.Uri.AbsolutePath == _fromUrl)
            {
                response.Redirect(_toUrl);
                return true;
            }

            return false;
        }
    }
}
