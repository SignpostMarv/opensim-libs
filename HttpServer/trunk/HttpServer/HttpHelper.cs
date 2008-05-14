using System;
using HttpServer;

namespace HttpServer
{
    /// <summary>
    /// Generic helper functions for Http
    /// </summary>
    public static class HttpHelper
    {
        public readonly static Uri EmptyUri = new Uri("http://localhost/");
        public const string HTTP10 = "HTTP/1.0";
        public const string HTTP11 = "HTTP/1.1";

        /// <summary>
        /// Parses a querystring.
        /// </summary>
        /// <param name="queryString">Querystring (url decoded)</param>
        /// <returns>A HttpInput object if successful; otherwise HttpInput.Empty</returns>
        public static HttpInput ParseQueryString(string queryString)
        {
            if (queryString == null)
                throw new ArgumentNullException("queryString");
            if (queryString == string.Empty)
                return HttpInput.Empty;

            int state = 0;
            int startpos = 0;
            string name = null;
            HttpInput input = new HttpInput("QueryString");
            for (int i = 0; i < queryString.Length; ++i)
            {
                char ch = queryString[i];

                if (state == 0 && ch == '=')
                {
                    name = queryString.Substring(startpos, i - startpos);
                    startpos = i + 1;
                    ++state;
                }
                else if (state == 1 && ch == '&')
                {
                    input.Add(name, queryString.Substring(startpos, i - startpos));
                    startpos = i + 1;
                    state = 0;
                    name = null;
                }
            }

            if (name != null && startpos < queryString.Length - 1)
                input.Add(name, queryString.Substring(startpos, queryString.Length - startpos));

            return input;
        }
    }
}
