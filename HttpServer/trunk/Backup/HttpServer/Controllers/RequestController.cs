using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HttpServer.Authentication;
using HttpServer.Exceptions;
using HttpServer.HttpModules;

namespace HttpServer.Controllers
{
    /// <summary>
    /// A controller in the Model-View-Controller pattern.
    ///  Derive this class and add method with one of the following signatures:
    /// "public string MethodName()" or "public void MyMethod()".
    /// 
    /// The first should return a string with the response, the latter
    /// should use SendHeader and SendBody methods to handle the response.
    /// </summary>
    /// <remarks>
    /// Last segment of the path is always broken into the properties Id and RequestedType
    /// Alhtough note that the RequestedType can also be empty if no file extension have
    /// been specified. A typical use of file extensions in controllers is to specify which type of
    /// format to return.
    /// </remarks>
    /// <example>
    /// public class MyController : RequestController
    /// {
    ///   public string Hello()
    ///   {
    ///       if (RequestedType == "xml")
    ///           return "&lt;hello&gt;World&lt;hello&gt;";
    ///       else
    ///           return "Hello " + Request.QueryString["user"].Value + ", welcome to my world";
    ///   }
    /// 
    ///   public void File()
    ///   {
    ///     Response.Headers.ContentType = "text/xml";
    ///     Response.SendHeader();
    ///   }
    /// }
    /// </example>
    /// <seealso cref="ControllerNameAttribute"/>
    /// <seealso cref="AuthRequiredAttribute"/>
    /// <seealso cref="AuthValidatorAttribute"/>
    public abstract class RequestController : HttpModule, ICloneable
    {
        private const string Html = "html";
        private readonly LinkedList<MethodInfo> _beforeFilters = new LinkedList<MethodInfo>();
        private readonly Dictionary<string, MethodInfo> _binaryMethods = new Dictionary<string, MethodInfo>();
        private readonly List<string> _authMethods = new List<string>();
        private readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        private LinkedListNode<MethodInfo> _betweenFilter = null;
        private string _controllerName;
        private MethodInfo _defaultMethod = null;
        private string _defaultMethodStr = null;
        private MethodInfo _authValidator = null;

        //used temp during method mapping.
        private string _id;
        private MethodInfo _method;
        private string _methodName;
        private HttpRequest _request;
        private string _requestedExtension;
        private HttpResponse _response;
        private HttpSession _session;

        public RequestController(RequestController controller)
        {
            _beforeFilters = controller._beforeFilters;
            _binaryMethods = controller._binaryMethods;
            _authMethods = controller._authMethods;
            _methods = controller._methods;
            _controllerName = controller.ControllerName;
            _defaultMethod = controller._defaultMethod;
            _defaultMethodStr = controller._defaultMethodStr;
            _authValidator = controller._authValidator;
        }

        public RequestController()
        {
            MapMethods();
        }

        /// <summary>
        /// object that was attached during http authentication process.
        /// </summary>
        /// <remarks>
        /// You can also assign this tag yourself if you are using regular
        /// http page login.
        /// </remarks>
        /// <seealso cref="AuthModule"/>
        protected object AuthenticationTag
        {
            get { return _session[AuthModule.AuthenticationTag]; }
            set { _session[AuthModule.AuthenticationTag] = value; }
        }

        public string ControllerName
        {
            get { return _controllerName; }
        }

        /// <summary>
        /// Specifies the method to use if no action have been specified.
        /// </summary>
        /// <exception cref="ArgumentException">If specified method do not exist.</exception>
        public string DefaultMethod
        {
            get { return _defaultMethodStr; }
            set
            {
                if (_methods.ContainsKey(value.ToLower()))
                {
                    _defaultMethodStr = value.ToLower();
                    _defaultMethod = _methods[_defaultMethodStr];
                }
                else if (_binaryMethods.ContainsKey(value.ToLower()))
                {
                    _defaultMethodStr = value.ToLower();
                    _defaultMethod = _binaryMethods[_defaultMethodStr];
                }
                else
                    throw new ArgumentException("New DefaultMethod value is not a valid controller method.");
            }
        }

        /// <summary>
        /// Id is the third part of the uri path.
        /// </summary>
        /// <remarks>
        /// Is extracted as in: /controller/action/id/
        /// </remarks>
        /// <example></example>
        protected string Id
        {
            get { return _id ?? string.Empty; }
        }

        /// <summary>
        /// Method currently being invoked.
        /// Always in lower case.
        /// </summary>
        public string MethodName
        {
            get { return _methodName; }
        }

        protected HttpRequest Request
        {
            get { return _request; }
        }

        /// <summary>
        /// Extension if a filename was specified.
        /// </summary>
        public string RequestedExtension
        {
            get { return _requestedExtension; }
        }

        protected HttpResponse Response
        {
            get { return _response; }
        }

        protected HttpSession Session
        {
            get { return _session; }
        }
        /*
        protected virtual void AddAuthAttribute(string methodName, object attribute)
        {
            if (attribute.GetType() == typeof (AuthenticatorAttribute))
            {
                AuthenticatorAttribute attrib = (AuthenticatorAttribute) attribute;
                try
                {
                    MethodInfo mi = GetType().GetMethod(attrib.Method);
                    if (methodName == ClassMethodName)
                        _classCheckAuthMethod = mi;
                    else
                        _authMethods.Add(methodName, mi);
                }
                catch (AmbiguousMatchException err)
                {
                    if (methodName == "class")
                        throw new InvalidOperationException(
                            "Failed to find Authenticator method for class " + GetType().Name, err);
                    else
                        throw new InvalidOperationException("Failed to find Authenticator method for " + GetType().Name +
                                                            "." + methodName);
                }
            }
            else
                throw new ArgumentException("Attribute is not of type AuthenticatorAttribute");
        }
        */

        /// <summary>
        /// Method that determines if an url should be handled or not by the module
        /// </summary>
        /// <param name="request">Url requested by the client.</param>
        /// <returns>true if module should handle the url.</returns>
        public virtual bool CanHandle(HttpRequest request)
        {
            if (request.UriParts.Length <= 0)
                return false;

            // check if controller name is correct. uri segments adds a slash to the segments
            if (string.Compare(request.UriParts[0], _controllerName, true) != 0)
                return false;

            // check action
            if (request.UriParts.Length > 1)
            {
                string uriPart = request.UriParts[1];
                int pos = uriPart.LastIndexOf('.');
                if (pos != -1)
                    uriPart = uriPart.Substring(0, pos);
                if (_methods.ContainsKey(uriPart) || _binaryMethods.ContainsKey(uriPart))
                    return true;
            }

            if (request.UriParts.Length == 1)
                return _defaultMethod != null;

            return false;
        }

        /// <summary>
        /// Determines which method to use.
        /// </summary>
        /// <param name="request">Requested resource</param>
        protected virtual MethodInfo GetMethod(HttpRequest request)
        {
            // Check where the default met
            if (request.UriParts.Length <= 1)
                return _defaultMethod;

            string uriPart = request.UriParts[1];
            int pos = uriPart.LastIndexOf('.');
            if (pos != -1)
            {
                _requestedExtension = uriPart.Substring(pos + 1);
                uriPart = uriPart.Substring(0, pos);
            }

            if (_methods.ContainsKey(uriPart))
                return _methods[uriPart];
            else if (_binaryMethods.ContainsKey(uriPart))
                return _binaryMethods[uriPart];


            return null;
        }

        /// <summary>
        /// Call all before filters
        /// </summary>
        /// <returns>true if a before filter wants to abort the processing.</returns>
        private bool InvokeBeforeFilters()
        {
            try
            {
                foreach (MethodInfo info in _beforeFilters)
                    if (!(bool) info.Invoke(this, null))
                        return true;

                return false;
            }
            catch (TargetInvocationException err)
            {
#if DEBUG
                FieldInfo remoteStackTraceString =
                    typeof(Exception).GetField("_remoteStackTraceString",
                                                BindingFlags.Instance | BindingFlags.NonPublic);
                remoteStackTraceString.SetValue(err.InnerException, err.InnerException.StackTrace + Environment.NewLine);
                throw err.InnerException;
#else
                    throw new InternalServerException("Controller filter failure, please try again.", err);
#endif
            }
        }

        protected void InvokeMethod()
        {
            if (_authMethods.Contains(_methodName))
            {
                if (_authValidator != null)
                {
                    bool res = (bool) _authValidator.Invoke(this, null);
                    if (!res)
                        throw new UnauthorizedException("Need to authenticate.");
                }
            }

            if (_method.ReturnType == typeof (string))
            {
                try
                {
                    string temp = (string) _method.Invoke(this, null);
                    if (temp != null)
                    {
                        TextWriter writer = new StreamWriter(Response.Body);
                        writer.Write(temp);
                        writer.Flush();
                    }
                }
                catch (TargetInvocationException err)
                {
#if DEBUG
                    FieldInfo remoteStackTraceString =
                        typeof (Exception).GetField("_remoteStackTraceString",
                                                    BindingFlags.Instance | BindingFlags.NonPublic);
                    remoteStackTraceString.SetValue(err.InnerException, err.InnerException.StackTrace + Environment.NewLine);
                    throw err.InnerException;
#else
                    throw new InternalServerException("Controller failure, please try again.", err);
#endif
                }
            }
            else
            {
                _method.Invoke(this, null);
            }
        }
        /*
        /// <summary>
        /// check authentication attributes for the class
        /// </summary>
        protected virtual void MapClassAuth()
        {
            object[] attributes = GetType().GetCustomAttributes(true);
            foreach (object attribute in attributes)
            {
                if (attribute.GetType() == typeof (AuthenticatorAttribute))
                    AddAuthAttribute(ClassMethodName, attribute);
                if (attribute.GetType() == typeof (AuthenticationRequiredAttribute))
                    AddCheckAuthAttribute(ClassMethodName, attribute);
            }
        }
        */
        /// <summary>
        /// This method goes through all methods in the controller and
        /// add's them to a dictionary. They are later used to invoke
        /// the correct method depending on the url
        /// </summary>
        private void MapMethods()
        {
            lock (_methods)
            {
                object[] controllerNameAttrs = GetType().GetCustomAttributes(typeof (ControllerNameAttribute), false);
                if (controllerNameAttrs.Length > 0)
                    _controllerName = ((ControllerNameAttribute)controllerNameAttrs[0]).Name;
                else
                {
                    _controllerName = GetType().Name;
                    if (ControllerName.Contains("Controller"))
                        _controllerName = ControllerName.Replace("Controller", "");
                    _controllerName = ControllerName.ToLower();
                }

                if (_methods.Count != 0)
                    return;

                MethodInfo[] methods =
                    GetType().GetMethods(BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance);
                foreach (MethodInfo info in methods)
                {
                    ParameterInfo[] parameters = info.GetParameters();

                    // find regular render methods
                    if (parameters.Length == 0 && info.ReturnType == typeof (string))
                    {
                        string name = info.Name.ToLower();
                        if (name.Length > 3 && (name.Substring(0, 4) == "get_" || name.Substring(0, 4) == "set_"))
                            continue;
                        if (name == "tostring")
                            continue;

                        // Add authenticators
                        object[] authAttributes = info.GetCustomAttributes(true);
                        foreach (object attribute in authAttributes)
                            if (attribute.GetType() == typeof (AuthRequiredAttribute))
                                _authMethods.Add(info.Name.ToLower());
                        _methods.Add(info.Name.ToLower(), info);
                    }

                    // find raw handlers 
                    object[] attributes = info.GetCustomAttributes(typeof (RawHandlerAttribute), true);
                    if (attributes.Length >= 1 && info.ReturnType == typeof (void) && parameters.Length == 0)
                    {
                        // Add authenticators
                        object[] authAttributes = info.GetCustomAttributes(true);
                        foreach (object attribute in authAttributes)
                            if (attribute.GetType() == typeof(AuthRequiredAttribute))
                                _authMethods.Add(info.Name.ToLower());
                        _binaryMethods.Add(info.Name.ToLower(), info);
                    }
                } //foreach

                methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (MethodInfo info in methods)
                {
                    ParameterInfo[] parameters = info.GetParameters();

                    // find before filters.
                    if (parameters.Length == 0 && info.ReturnType == typeof (bool))
                    {
                        object[] authAttributes = info.GetCustomAttributes(true);
                        foreach (object attribute in authAttributes)
                            if (attribute.GetType() == typeof(AuthValidatorAttribute))
                            {
                                if (_authValidator != null)
                                    throw new InvalidOperationException("Auth validator have already been specified.");
                                _authValidator = info;
                            }
                            else if (attribute.GetType() == typeof (BeforeFilterAttribute))
                            {
                                BeforeFilterAttribute attr = (BeforeFilterAttribute) attribute;
                                LinkedListNode<MethodInfo> node = new LinkedListNode<MethodInfo>(info);

                                if (_betweenFilter == null)
                                    _betweenFilter = node;

                                if (attr.Position == FilterPosition.First)
                                    _beforeFilters.AddFirst(node);
                                else if (attr.Position == FilterPosition.Last)
                                    _beforeFilters.AddLast(node);
                                else
                                {
                                    _beforeFilters.AddAfter(_betweenFilter, node);
                                    _betweenFilter = node;
                                }
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Method that process the url
        /// </summary>
        /// <param name="request">Uses Uri and QueryString to determine method.</param>
        /// <param name="response">Relays response object to invoked method.</param>
        /// <param name="session">Relays session object to invoked method. </param>
        public override bool Process(HttpRequest request, HttpResponse response, HttpSession session)
        {
            if (!CanHandle(request))
                return false;

            SetupRequest(request, response, session);

            if (InvokeBeforeFilters())
                return true;

            InvokeMethod();

            return true;
        }

        /// <summary>
        /// Will assign all variables that are unique for each session
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="session"></param>
        protected virtual void SetupRequest(HttpRequest request, HttpResponse response, HttpSession session)
        {
            _requestedExtension = Html;

            // extract id
            if (request.Uri.Segments.Length > 3)
            {
                _id = request.Uri.Segments[3];
                if (_id.EndsWith("/"))
                    _id = _id.Substring(0, _id.Length - 1);
                else
                {
                    int pos = _id.LastIndexOf('.');
                    if (pos != -1)
                    {
                        _requestedExtension = _id.Substring(pos + 1);
                        _id = _id.Substring(0, pos);
                    }
                }
            }
            else if (request.QueryString["id"] != HttpInputItem.Empty)
                _id = request.QueryString["id"].Value;

            _request = request;
            _response = response;
            _session = session;

            if (request.Uri.Segments.Length == 2 && _defaultMethod == null)
                throw new NotFoundException("No default method is specified.");

            _method = GetMethod(request);
            if (_method == null)
                throw new NotFoundException("Requested action could not be found.");

            _methodName = _method.Name.ToLower();
        }

        #region ICloneable Members

        public abstract object Clone();

        #endregion
    }
}