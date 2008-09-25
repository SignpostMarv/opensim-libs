using System;
using System.Web;

namespace HttpServer
{
    /// <summary>
    /// Generic helper functions for Http
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// Version string for HTTP v1.0
        /// </summary>
        public const string HTTP10 = "HTTP/1.0";

        /// <summary>
        /// Version string for HTTP v1.1
        /// </summary>
        public const string HTTP11 = "HTTP/1.1";

        /// <summary>
        /// An empty url
        /// </summary>
        public static readonly Uri EmptyUri = new Uri("http://localhost/");

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
                int newIndexPos;
                if (state == 0 && IsEqual(queryString, ref i, out newIndexPos))
                {
                    name = queryString.Substring(startpos, i - startpos);
                    i = newIndexPos;
                    startpos = i + 1;
                    ++state;
                }
                else if (state == 1 && IsAmp(queryString, ref i, out newIndexPos))
                {
                    Add(input, name, queryString.Substring(startpos, i - startpos));
                    i = newIndexPos;
                    startpos = i + 1;
                    state = 0;
                    name = null;
                }
            }

            if (state == 0 && !input.GetEnumerator().MoveNext())
                throw new ArgumentException("Not a valid querystring: " + queryString);

            if (name != null && startpos < queryString.Length)
                Add(input, name, queryString.Substring(startpos, queryString.Length - startpos));

            return input;
        }

        private static bool IsEqual(string queryStr, ref int index, out int outIndex)
        {
            outIndex = index;
            if (queryStr[index] == '=')
                return true;
            if (queryStr[index] == '%' && queryStr.Length > index + 2 && queryStr[index + 1] == '3'
                && (queryStr[index + 2] == 'd' || queryStr[index + 2] == 'D'))
            {
                outIndex += 2;
                return true;
            }
            return false;
        }

        private static bool IsAmp(string queryStr, ref int index, out int outIndex)
        {
            outIndex = index;
            if (queryStr[index] == '%' && queryStr.Length > index + 2 && queryStr[index + 1] == '2' &&
                queryStr[index + 2] == '6')
                outIndex += 2;
            else if (queryStr[index] == '&')
            {
                if (queryStr.Length > index + 4
                    && (queryStr[index + 1] == 'a' || queryStr[index + 1] == 'A')
                    && (queryStr[index + 2] == 'm' || queryStr[index + 2] == 'M')
                    && (queryStr[index + 3] == 'p' || queryStr[index + 3] == 'P')
                    && queryStr[index + 4] == ';')
                    outIndex += 4;
            }
            else
                return false;

            return true;
        }

        private static void Add(IHttpInput input, string name, string value)
        {
            input.Add(HttpUtility.UrlDecode(name), HttpUtility.UrlDecode(value));
        }
    }
}
