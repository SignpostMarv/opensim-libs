using System;
using System.Net;

namespace HttpServer.Exceptions
{
    public class NotFoundException : HttpException
    {
        public NotFoundException(string message, Exception inner) : base(HttpStatusCode.NotFound, message, inner)
        {
        }

        public NotFoundException(string message)
            : base(HttpStatusCode.NotFound, message)
        {
        }
    }
}
