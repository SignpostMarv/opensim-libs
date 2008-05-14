using System;
using System.Net;

namespace HttpServer.Exceptions
{
    public class HttpException : Exception
    {
        private HttpStatusCode _code;

        public HttpException(HttpStatusCode code, string message) : base(code + ": " + message)
        {
            _code = code;
        }

        public HttpException(HttpStatusCode code, string message, Exception inner)
            : base(code + ": " + message, inner)
        {
            _code = code;
        }

        public HttpStatusCode HttpStatusCode
        {
            get { return _code; }
        }
    }
}