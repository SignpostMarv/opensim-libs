using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using HttpServer.Authentication;
using HttpServer.Exceptions;
using HttpServer.FormDecoders;
using HttpServer.HttpModules;
using HttpServer.Sessions;

namespace HttpServer
{
    public delegate string RealmHandler(string domain);

    /// <summary>
    /// The Tiny framework web server is a bit different.
    /// It can only speak HTTP, but can not handle anything itself.
    /// You need to add a website module to it, or use a controller module in it.
    /// </summary>
    /// <example>
    /// // this small example will add two web site modules, thus handling
    /// // two different sites. In reality you should add controller modules or something
    /// // two the website modules to be able to handle different requests.
    /// HttpServer server = new HttpServer();
    /// server.Add(new WebSiteModule("www.gauffin.com", "Gauffin Telecom AB"));
    /// server.Add(new WebSiteModule("www.vapadi.se", "Remote PBX"));
    /// 
    /// // start regular http
    /// server.Start(IPAddress.Any, 80);
    /// 
    /// // start https
    /// server.Start(IPAddress.Any, 443, myCertificate);
    /// </example>
    public class HttpServer
    {
        private readonly FormDecoderProvider _formDecodersProvider = new FormDecoderProvider();
        private readonly List<HttpModule> _modules = new List<HttpModule>();
        private readonly List<RedirectRule> _rules = new List<RedirectRule>();
        private readonly List<AuthModule> _authModules = new List<AuthModule>();
        private HttpListener _httpListener;
        private HttpListener _httpsListener;
        private string _serverName = "TinyServer";
        private string _sessionCookieName = "__tiny_sessid";
        private HttpSessionStore _sessionStore;

        /// <summary>
        /// Modules used for authentication. The module that is is added first is used as 
        /// the default auth module.
        /// </summary>
        /// <remarks>Use the corresponding property
        /// in the WebSiteModule if you are using multiple websites.</remarks>
        public IList<AuthModule> AuthenticationModules
        {
            get { return _authModules; }
        }

        /// <summary>
        /// Form decoder providers are used to decode request body (which normally contains form data).
        /// </summary>
        public FormDecoderProvider FormDecoderProviders
        {
            get { return _formDecodersProvider; }
        }

        /// <summary>
        /// Server name sent in http responses.
        /// </summary>
        /// <remarks>
        /// Do NOT include version in name, since it makes it 
        /// easier for hackers.
        /// </remarks>
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        /// <summary>
        /// Name of cookie where session id is stord.
        /// </summary>
        public string SessionCookieName
        {
            get { return _sessionCookieName; }
            set { _sessionCookieName = value; }
        }

        public void Add(RedirectRule rule)
        {
            _rules.Add(rule);
        }

        public void Add(HttpModule module)
        {
            _modules.Add(module);
        }

        protected virtual void DecodeBody(HttpRequest request)
        {
            try
            {
                if (request.Body.Length > 0)
                    request.DecodeBody(_formDecodersProvider);
            }
            catch (InvalidOperationException err)
            {
                throw new InternalServerException("Failed to decode form data.", err);
            }
            catch (InvalidDataException err)
            {
                throw new InternalServerException("Form contains invalid format.", err);
            }
        }

        protected virtual void ErrorPage(HttpResponse response, HttpStatusCode error, string defaultReason)
        {
            response.Reason = defaultReason;
            response.Status = error;

            StreamWriter writer = new StreamWriter(response.Body);
            writer.WriteLine(defaultReason);
            writer.Flush();
        }

        protected virtual string GetRealm(HttpRequest request)
        {
            if (RealmWanted != null)
                return RealmWanted(request.Headers["host"] ?? "localhost");
            else
                return ServerName;
        }

        protected virtual void HandleRequest(HttpClientContext context, HttpRequest request, HttpResponse response,
                                             HttpSession session)
        {
            bool handled = false;
            try
            {
                DecodeBody(request);
                if (ProcessAuthentication(request, response, session))
                {
                    foreach (HttpModule module in _modules)
                    {
                        if (module.Process(request, response, session))
                            handled = true;
                    }
                }
            }
            catch (HttpException err)
            {
                if (err.HttpStatusCode == HttpStatusCode.Unauthorized)
                {
                    AuthModule mod;
                    lock (_authModules)
                        if (_authModules.Count > 0)
                            mod = _authModules[0];
                        else
                            mod = null;

                    if (mod != null)
                        RequestAuthentication(mod, request, response);
                }
                else
                    ErrorPage(response, err.HttpStatusCode, err.Message);
            }

            if (!handled && response.Status == HttpStatusCode.OK)
                ErrorPage(response, HttpStatusCode.NotFound, "Resource not found");

            if (!response.HeadersSent)
            {
                // Dispose session if it was not used.
                if (session.Count > 0)
                {
                    _sessionStore.Save(session);
                    if (response.Cookies[_sessionCookieName] == null)
                        response.Cookies.Add(new ResponseCookie(_sessionCookieName, session.Id, DateTime.Now.AddMinutes(20).AddDays(1)));
                }
                else
                    _sessionStore.AddUnused(session);

                // Now add cookies
                foreach (ResponseCookie cookie in response.Cookies)
                    response.AddHeader("Set-Cookie", cookie.ToString());
            }

            if (!response.Sent)
                response.Send();
        }

        private void Init()
        {
            if (_sessionStore == null)
            {
                WriteLog(this, LogPrio.Info, "Defaulting to memory session store.");
                _sessionStore = new MemorySessionStore();
            }

            // add default decoders if none have been added.
            if (_formDecodersProvider.Count == 0)
            {
                WriteLog(this, LogPrio.Info, "Loading all default form decoders, since none have been added.");
                _formDecodersProvider.Add(new UrlDecoder());
                _formDecodersProvider.Add(new MultipartFormDecoder());
                _formDecodersProvider.Add(new XmlDecoder());
            }
        }

        protected virtual void OnClientDisconnected(HttpClientContext client, SocketError error)
        {
        }

        /// <summary>
        /// Handle authentication
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="session"></param>
        /// <returns>true if request can be handled; false if not.</returns>
        protected virtual bool ProcessAuthentication(HttpRequest request, HttpResponse response, HttpSession session)
        {
            if (_authModules.Count > 0)
            {
                bool authenticate = false;
                object authTag = null;
                if (request.Headers["authorization"] != null)
                {
                    authenticate = true;
                    string authHeader = request.Headers["authorization"];
                    int pos = authHeader.IndexOf(' ');                    
                    if (pos == -1)
                        throw new BadRequestException("Invalid authorization header");

                    // first word identifies the type of authentication to use.
                    string word = authHeader.Substring(0, pos).ToLower();

                    // find the mod to use.
                    AuthModule mod = null;
                    lock (_authModules)
                    {
                        foreach (AuthModule aModule in _authModules)
                        {
                            if (aModule.Name == word)
                            {
                                mod = aModule;
                                break;
                            }
                        }
                    }
                    if (mod != null)
                    {
                        authTag = mod.Authenticate(authHeader, GetRealm(request), request.Method);
                        session[AuthModule.AuthenticationTag] = authTag;
                    }
                }

                
                // Check if auth is needed.
                if (authTag == null)
                {
                    lock (_authModules)
                    {
                        foreach (AuthModule module in _authModules)
                        {
                            if (module.AuthenticationRequired(request))
                            {
                                RequestAuthentication(module, request, response);
                                return false;
                            }
                        }

                        // modules can have inited the authentication
                        // and then the module.AuthenticationRequired method will not have been used.
                        if (authenticate && _authModules.Count > 0)
                        {
                            RequestAuthentication(_authModules[0], request, response);
                            return false;
                        }
                    }
                }

            }

            return true;
        }

        /// <summary>
        /// Will request authentication.
        /// </summary>
        /// <remarks>
        /// Sends respond to client, nothing else can be done with the response after this.
        /// </remarks>
        /// <param name="mod"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        protected virtual void RequestAuthentication(AuthModule mod, HttpRequest request, HttpResponse response)
        {
            string theResponse = mod.CreateResponse(GetRealm(request));
            response.AddHeader("www-authenticate", theResponse);
            response.Reason = "Authentication required.";
            response.Status = HttpStatusCode.Unauthorized;
        }

        private void SetupRequest(HttpClientContext context, HttpRequest request)
        {
            HttpResponse response = new HttpResponse(context, request);
            try
            {
                foreach (RedirectRule rule in _rules)
                {
                    if (rule.Process(request, response))
                    {
                        response.Send();
                        return;
                    }
                }

                RequestCookies cookies;
                // load cookies if the exist.
                if (request.Headers["cookie"] != null)
                    cookies = new RequestCookies(request.Headers["cookie"]);
                else
                    cookies = new RequestCookies(string.Empty);

                request.SetCookies(cookies);

                HttpSession session;
                if (cookies[_sessionCookieName] != null)
                {
                    session = _sessionStore.Load(cookies[_sessionCookieName].Value);
                    if (session == null)
                        session = _sessionStore.Create(cookies[_sessionCookieName].Value);
                }
                else
                    session = _sessionStore.Create();

                HandleRequest(context, request, response, session);
            }
            catch (Exception err)
            {
                if (ExceptionThrown == null)
#if DEBUG
                    throw;
#else
                    WriteLog(LogPrio.Fatal, err.Message);
#endif
                else
                {
                    ExceptionThrown(this, err);
                    try
                    {
#if DEBUG
                        context.Respond("HTTP/1.0", HttpStatusCode.InternalServerError,  "Internal server error", err.ToString());
#else
                        context.Respond("HTTP/1.0", HttpStatusCode.InternalServerError,  "Internal server error");
#endif
                    }
                    catch(Exception){}
                }
            }
        }

        /// <summary>
        /// Start the webserver using regular HTTP.
        /// </summary>
        /// <param name="address">IP Address to listen on, use IpAddress.Any to accept connections on all ipaddresses/network cards.</param>
        /// <param name="port">Port to listen on. 80 can be a good idea =)</param>
        public void Start(IPAddress address, int port)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            if (port <= 0)
                throw new ArgumentException("Port must be a positive number.");

            if (_httpListener == null)
            {
                _httpListener = new HttpListener(address, port);
                _httpListener.LogWriter = LogEntryWritten;
                _httpListener.RequestHandler = SetupRequest;
                _httpListener.Start(5);
                Init();
            }
        }

        /// <summary>
        /// Accept secure connections.
        /// </summary>
        /// <param name="address">IP Address to listen on, use IpAddress.Any to accept connections on all ipaddresses/network cards.</param>
        /// <param name="port">Port to listen on. 80 can be a good idea =)</param>
        /// <param name="certificate">Certificate to use</param>
        public void Start(IPAddress address, int port, X509Certificate certificate)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            if (port <= 0)
                throw new ArgumentException("Port must be a positive number.");

            if (_httpsListener == null)
            {
                _httpsListener = new HttpListener(address, port, certificate);
                _httpsListener.LogWriter = LogEntryWritten;
                _httpsListener.RequestHandler = SetupRequest;
                _httpsListener.Start(5);
                Init();
            }
        }

        public void Stop()
        {
            if (_httpListener != null)
            {
                _httpListener.Stop();
                _httpListener = null;
            }
            if (_httpsListener != null)
            {
                _httpsListener.Stop();
                _httpsListener = null;
            }
        }

        protected virtual void WriteLog(LogPrio prio, string message)
        {
            WriteLog(this, prio, message);
        }

        internal void WriteLog(object source, LogPrio prio, string message)
        {
            if (LogEntryWritten != null)
                LogEntryWritten(this, prio, message);
            Console.WriteLine(source.GetType().Name + ": " + prio + ", " + message);
        }

        /// <summary>
        /// Use this event to be able to get log entries into your favorite 
        /// log library.
        /// </summary>
        public event WriteLogHandler LogEntryWritten;

        /// <summary>
        /// Realms are used during http authentication.
        /// Default realm is same as server name.
        /// </summary>
        public event RealmHandler RealmWanted;

        /// <summary>
        /// Let's to receive unhandled exceptions from the threads.
        /// </summary>
        /// <remarks>
        /// Exceptions will be thrown during debug mode if this event is not used,
        /// exceptions will be printed to console and supressed during release mode.
        /// </remarks>
        public event ExceptionHandler ExceptionThrown;
    }
}