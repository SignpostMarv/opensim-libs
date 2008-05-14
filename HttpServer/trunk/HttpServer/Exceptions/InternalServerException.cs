using System;
using System.Net;

namespace HttpServer.Exceptions
{
    /// <summary>
    /// The server encountered an unexpected condition which prevented it from fulfilling the request.
    /// </summary>
    public class InternalServerException : HttpException
    {
        public InternalServerException()
            : base(HttpStatusCode.InternalServerError, "The server encountered an unexpected condition which prevented it from fulfilling the request.")
        {
        }

        public InternalServerException(string message)
            : base(HttpStatusCode.InternalServerError, message)
        {
        }

        public InternalServerException(string message, Exception inner)
            : base(HttpStatusCode.InternalServerError, message, inner)
        {
        }
    }
}
