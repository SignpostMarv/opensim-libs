using System;

namespace HttpServer.Controllers
{
    /// <summary>
    /// Marks methods to let framework know that the method is protected
    /// </summary>
    public class AuthRequiredAttribute : Attribute
    {
    }
}
