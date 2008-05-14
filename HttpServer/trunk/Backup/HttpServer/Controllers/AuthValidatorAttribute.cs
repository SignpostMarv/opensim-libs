using System;
using HttpServer;
using HttpServer.HttpModules;

namespace HttpServer.Controllers
{
    /// <summary>
    /// Method marked with this attribute determines if authentication is required.
    /// </summary>
    /// <seealso cref="ControllerModule"/>
    /// <seealso cref="HttpServer"/>
    /// <seealso cref="AuthRequiredAttribute"/>
    /// <seealso cref="WebSiteModule"/>
    public class AuthValidatorAttribute : Attribute
    {
    }
}
