using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HttpServer.Exceptions;
using HttpServer.Parser;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


namespace HttpServer
{
    /// <summary>
    /// Contains a connection to a browser/client.
    /// </summary>
    /// <remarks>
    /// Remember to <see cref="Start"/> after you have hooked the <see cref="RequestReceived"/> event.
    /// </remarks>
    /// TODO: Maybe this class should be broken up into HttpClientChannel and HttpClientContext?
    public class HttpClientContext : IHttpClientContext, IDisposable
    {
        const int MAXREQUESTS = 20;
        const int MAXKEEPALIVE = 60000;

        static private int basecontextID;

        private readonly byte[] _buffer;
        private int _bytesLeft;
        private ILogWriter _log;
        private readonly IHttpRequestParser _parser;
        private readonly int _bufferSize;
        private IHttpRequest _currentRequest;
        private HashSet<uint> requestsInServiceIDs;
        private Socket _sock;

        public bool Available = true;
        public bool StreamPassedOff = false;
        public int MonitorStartMS = 0;
        public int MonitorKeepaliveMS = 0;
        public bool TriggerKeepalive = false;
        public int TimeoutFirstLine = 70000; // 70 seconds
        public int TimeoutRequestReceived = 180000; // 180 seconds

        // The difference between this and request received is on POST more time is needed before we get the full request.
        public int TimeoutFullRequestProcessed = 600000; // 10 minutes
        public int m_TimeoutKeepAlive = MAXKEEPALIVE; // 400 seconds before keepalive timeout
        // public int TimeoutKeepAlive = 120000; // 400 seconds before keepalive timeout

        public int m_MAXRequests = MAXREQUESTS;

        public bool FirstRequestLineReceived;
        public bool FullRequestReceived;
        public bool FullRequestProcessed;

        private bool gotResponseClose = false;
        private bool isSendingResponse = false;

        public int contextID { get; private set; }
        public int TimeoutKeepAlive
        {
            get { return m_TimeoutKeepAlive; }
            set
            {
                if (value > MAXKEEPALIVE)
                    m_TimeoutKeepAlive = MAXKEEPALIVE;
                else
                    m_TimeoutKeepAlive = value;
            }
        }

        public int MAXRequests
        {
            get { return m_MAXRequests; }
            set
            {
                if (value > MAXREQUESTS)
                    m_MAXRequests = MAXREQUESTS;
                else if (value <= 0)
                    m_MAXRequests = 0;
                else
                    m_MAXRequests = value;
            }
        }

        public bool IsSending()
        {
            return isSendingResponse;
        }

        public bool StopMonitoring;


        /// <summary>
        /// Context have been started (a new client have connected)
        /// </summary>
        public event EventHandler Started = delegate { };

        /// <summary>
		/// Initializes a new instance of the <see cref="HttpClientContext"/> class.
        /// </summary>
        /// <param name="secured">true if the connection is secured (SSL/TLS)</param>
        /// <param name="remoteEndPoint">client that connected.</param>
        /// <param name="stream">Stream used for communication</param>
        /// <param name="parserFactory">Used to create a <see cref="IHttpRequestParser"/>.</param>
        /// <param name="bufferSize">Size of buffer to use when reading data. Must be at least 4096 bytes.</param>
        /// <exception cref="SocketException">If <see cref="Socket.BeginReceive(byte[],int,int,SocketFlags,AsyncCallback,object)"/> fails</exception>
        /// <exception cref="ArgumentException">Stream must be writable and readable.</exception>
        public HttpClientContext(bool secured, IPEndPoint remoteEndPoint,
                                    Stream stream, IRequestParserFactory parserFactory, int bufferSize, Socket sock)
        {
            Check.Require(remoteEndPoint, "remoteEndPoint");
            Check.NotEmpty(remoteEndPoint.Address.ToString(), "remoteEndPoint.Address");
            Check.Require(stream, "stream");
            Check.Require(parserFactory, "parser");
            Check.Require(sock, "socket");

            if (!stream.CanWrite || !stream.CanRead)
                throw new ArgumentException("Stream must be writable and readable.");

            _bufferSize = 8192;
            RemoteAddress = remoteEndPoint.Address.ToString();
            RemotePort = remoteEndPoint.Port.ToString();
            _log = NullLogWriter.Instance;
            _parser = parserFactory.CreateParser(_log);
            _parser.RequestCompleted += OnRequestCompleted;
            _parser.RequestLineReceived += OnRequestLine;
            _parser.HeaderReceived += OnHeaderReceived;
            _parser.BodyBytesReceived += OnBodyBytesReceived;
            _currentRequest = new HttpRequest(this);
            Available = false;
            IsSecured = secured;
            _stream = stream;
            _sock = sock;
            _buffer = new byte[bufferSize];
            requestsInServiceIDs = new HashSet<uint>();

            SSLCommonName = "";
            if (secured)
            {
                SslStream _ssl = (SslStream)_stream;
                X509Certificate _cert1 = _ssl.RemoteCertificate;
                if (_cert1 != null)
                {
                    X509Certificate2 _cert2 = new X509Certificate2(_cert1);
                    if (_cert2 != null)
                        SSLCommonName = _cert2.GetNameInfo(X509NameType.SimpleName, false);
                }
            }

            basecontextID++;
            if (basecontextID < 0)
                basecontextID = 1;

            contextID = basecontextID;
        }

        public bool CanSend()
        {
            if (Available || contextID < 0)
                return false;

            if (Stream == null || _sock == null || !_sock.Connected)
                return false;

            return true;
        }

        /// <summary>
        /// Process incoming body bytes.
        /// </summary>
        /// <param name="sender"><see cref="IHttpRequestParser"/></param>
        /// <param name="e">Bytes</param>
        protected virtual void OnBodyBytesReceived(object sender, BodyEventArgs e)
        {
            _currentRequest.AddToBody(e.Buffer, e.Offset, e.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHeaderReceived(object sender, HeaderEventArgs e)
        {
            if (string.Compare(e.Name, "expect", true) == 0 && e.Value.Contains("100-continue"))
            {
                lock (requestsInServiceIDs)
                {
                    if (requestsInServiceIDs.Count == 0)
                        Respond("HTTP/1.1", HttpStatusCode.Continue, "Please continue mate.");
                }
            }

            _currentRequest.AddHeader(e.Name, e.Value);
        }

        private void OnRequestLine(object sender, RequestLineEventArgs e)
        {
            _currentRequest.Method = e.HttpMethod;
            _currentRequest.HttpVersion = e.HttpVersion;
            _currentRequest.UriPath = e.UriPath;
            _currentRequest.AddHeader("remote_addr", RemoteAddress);
            _currentRequest.AddHeader("remote_port", RemotePort);
            FirstRequestLineReceived = true;
        }

        /// <summary>
        /// Overload to specify own type.
        /// </summary>
        /// <remarks>
        /// Must be specified before the context is being used.
        /// </remarks>
        protected IHttpRequest CurrentRequest
        {
            get
            {
                return _currentRequest;
            }
            set
            {
                _currentRequest = value;
            }
        }

        /// <summary>
        /// Start reading content.
        /// </summary>
        /// <remarks>
        /// Make sure to call base.Start() if you override this method.
        /// </remarks>
        public virtual void Start()
        {
            try
            {
                _stream.BeginRead(_buffer, 0, _bufferSize, OnReceive, null);
            }
            catch (IOException err)
            {
                LogWriter.Write(this, LogPrio.Debug, err.ToString());
            }

            Started(this, EventArgs.Empty);
        }

        /// <summary>
        /// Clean up context.
        /// </summary>
        /// <remarks>
        /// Make sure to call base.Cleanup() if you override the method.
        /// </remarks>
        public virtual void Cleanup()
        {
            if (StreamPassedOff)
                return;

            if (Stream != null)
            {
                Stream.Close();
                Stream = null;
                _sock = null;
            }
            _currentRequest.Clear();
            requestsInServiceIDs.Clear();
            _bytesLeft = 0;

            FirstRequestLineReceived = false;
            FullRequestReceived = false;
            FullRequestProcessed = false;
            MonitorStartMS = 0;
            StopMonitoring = true;
            MonitorKeepaliveMS = 0;
            TriggerKeepalive = false;
            gotResponseClose = false;
            isSendingResponse = false;

            contextID = -100;
            _parser.Clear();
        }

        public void Close()
        {
            Cleanup();
            Available = true;
        }

        /// <summary>
        /// Using SSL or other encryption method.
        /// </summary>
        [Obsolete("Use IsSecured instead.")]
        public bool Secured
        {
            get { return IsSecured; }
        }

        /// <summary>
        /// Using SSL or other encryption method.
        /// </summary>
        public bool IsSecured { get; internal set; }


        // returns the SSL commonName of remote Certificate
        public string SSLCommonName { get; internal set; }

        /// <summary>
        /// Specify which logger to use.
        /// </summary>
        public ILogWriter LogWriter
        {
            get { return _log; }
            set
            {
                _log = value ?? NullLogWriter.Instance;
                _parser.LogWriter = _log;
            }
        }

        private Stream _stream;

        /// <summary>
        /// Gets or sets the network stream.
        /// </summary>
        internal Stream Stream
        {
            get { return _stream; }
            set { _stream = value; }
        }

        /// <summary>
        /// Gets or sets IP address that the client connected from.
        /// </summary>
        internal string RemoteAddress { get; set; }

        /// <summary>
        /// Gets or sets port that the client connected from.
        /// </summary>
        internal string RemotePort { get; set; }

        /// <summary>
        /// Disconnect from client
        /// </summary>
        /// <param name="error">error to report in the <see cref="Disconnected"/> event.</param>
        public void Disconnect(SocketError error)
        {
            // disconnect may not throw any exceptions
            try
            {
                try
                {
                    if (Stream != null)
                    {
                        if (error == SocketError.Success)
                            Stream.Flush();
                        Stream.Close();
                        Stream = null;
                    }
                    _sock = null;
                }
                catch { }

                Disconnected(this, new DisconnectedEventArgs(error));
            }
            catch (Exception err)
            {
                LogWriter.Write(this, LogPrio.Error, "Disconnect threw an exception: " + err);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                int bytesRead = 0;
                if (Stream == null)
                    return;
                try
                {
                    bytesRead = Stream.EndRead(ar);
                }
                catch (NullReferenceException)
                {
                    Disconnect(SocketError.ConnectionReset);
                    return;
                }

                if (bytesRead == 0)
                {
                    Disconnect(SocketError.ConnectionReset);
                    return;
                }
                _bytesLeft += bytesRead;
                if (_bytesLeft > _buffer.Length)
                {
#if DEBUG
                    throw new BadRequestException("Too large HTTP header: " + Encoding.UTF8.GetString(_buffer, 0, bytesRead));
#else
                    throw new BadRequestException("Too large HTTP header: " + _bytesLeft);
#endif
                }

#if DEBUG
#pragma warning disable 219
                string temp = Encoding.ASCII.GetString(_buffer, 0, _bytesLeft);
                LogWriter.Write(this, LogPrio.Trace, "Received: " + temp);
#pragma warning restore 219
#endif
                int offset = _parser.Parse(_buffer, 0, _bytesLeft);
                if (Stream == null)
                    return; // "Connection: Close" in effect.

                // try again to see if we can parse another message (check parser to see if it is looking for a new message)
                int nextOffset;
                int nextBytesleft = _bytesLeft - offset;
                //                while (_parser.CurrentState == RequestParserState.FirstLine && offset != 0 && _bytesLeft - offset > 0)
                while (offset != 0 && nextBytesleft > 0)
                {
#if DEBUG
                    temp = Encoding.ASCII.GetString(_buffer, offset, nextBytesleft);
                    LogWriter.Write(this, LogPrio.Trace, "Processing: " + temp);
#endif

                    nextOffset = _parser.Parse(_buffer, offset, nextBytesleft);

                    if (Stream == null)
                        return; // "Connection: Close" in effect.

                    if (nextOffset == 0)
                        break;

                    offset = nextOffset;
                    nextBytesleft = _bytesLeft - offset;
                }

                // copy unused bytes to the beginning of the array
                if (offset > 0 && _bytesLeft > offset)
                    Buffer.BlockCopy(_buffer, offset, _buffer, 0, _bytesLeft - offset);

                _bytesLeft -= offset;
                if (Stream != null && Stream.CanRead)
                    if (!StreamPassedOff)
                        Stream.BeginRead(_buffer, _bytesLeft, _buffer.Length - _bytesLeft, OnReceive, null);
                    else
                    {
                        _log.Write(this, LogPrio.Warning, "Could not read any more from the socket.");
                        Disconnect(SocketError.Success);
                    }
            }
            catch (BadRequestException err)
            {
                LogWriter.Write(this, LogPrio.Warning, "Bad request, responding with it. Error: " + err);
                try
                {
                    Respond("HTTP/1.0", HttpStatusCode.BadRequest, err.Message);
                }
                catch (Exception err2)
                {
                    LogWriter.Write(this, LogPrio.Fatal, "Failed to reply to a bad request. " + err2);
                }
                Disconnect(SocketError.NoRecovery);
            }
            catch (IOException err)
            {
                LogWriter.Write(this, LogPrio.Debug, "Failed to end receive: " + err.Message);
                if (err.InnerException is SocketException)
                    Disconnect((SocketError)((SocketException)err.InnerException).ErrorCode);
                else
                    Disconnect(SocketError.ConnectionReset);
            }
            catch (ObjectDisposedException err)
            {
                LogWriter.Write(this, LogPrio.Debug, "Failed to end receive : " + err.Message);
                Disconnect(SocketError.NotSocket);
            }
            catch (NullReferenceException err)
            {
                LogWriter.Write(this, LogPrio.Debug, "Failed to end receive : NullRef: " + err.Message);
                Disconnect(SocketError.NoRecovery);
            }
            catch (Exception err)
            {
                LogWriter.Write(this, LogPrio.Debug, "Failed to end receive: " + err.Message);
                Disconnect(SocketError.NoRecovery);
            }
        }

        private void OnRequestCompleted(object source, EventArgs args)
        {
            TriggerKeepalive = false;
            MonitorKeepaliveMS = 0;

            // load cookies if they exist
            RequestCookies cookies = _currentRequest.Headers["cookie"] != null
                ? new RequestCookies(_currentRequest.Headers["cookie"])
                : new RequestCookies(String.Empty);
            _currentRequest.SetCookies(cookies);

            _currentRequest.Body.Seek(0, SeekOrigin.Begin);

            FullRequestReceived = true;

            int nreqs;
            lock (requestsInServiceIDs)
            {
                nreqs = requestsInServiceIDs.Count;
                requestsInServiceIDs.Add(_currentRequest.ID);
                if (m_MAXRequests > 0)
                    m_MAXRequests--;
            }

            // for now pipeline requests need to be serialized by opensim
            RequestReceived(this, new RequestEventArgs(_currentRequest));

            _currentRequest = new HttpRequest(this);

            int nreqsnow;
            lock (requestsInServiceIDs)
            {
                nreqsnow = requestsInServiceIDs.Count;
            }
            if (nreqs != nreqsnow)
            {
                // request was not done by us
            }
        }

        public void ReqResponseAboutToSend(uint requestID)
        {
            isSendingResponse = true;
        }

        public void ReqResponseSent(uint requestID, ConnectionType ctype)
        {
            if (ctype == ConnectionType.Close)
                gotResponseClose = true;
            else
            {
                // breakpoint
            }

            bool doclose = gotResponseClose;
            lock (requestsInServiceIDs)
            {
                requestsInServiceIDs.Remove(requestID);
//                doclose = doclose && requestsInServiceIDs.Count == 0;
                if (requestsInServiceIDs.Count > 1)
                {

                }
            }

            isSendingResponse = false;
            if (doclose)
                Disconnect(SocketError.Success);
            else
            {
                lock (requestsInServiceIDs)
                {
                    if (requestsInServiceIDs.Count == 0)
                        TriggerKeepalive = true;
                }
            }
        }

        /// <summary>
        /// Send a response.
        /// </summary>
        /// <param name="httpVersion">Either <see cref="HttpHelper.HTTP10"/> or <see cref="HttpHelper.HTTP11"/></param>
        /// <param name="statusCode">HTTP status code</param>
        /// <param name="reason">reason for the status code.</param>
        /// <param name="body">HTML body contents, can be null or empty.</param>
        /// <param name="contentType">A content type to return the body as, i.e. 'text/html' or 'text/plain', defaults to 'text/html' if null or empty</param>
        /// <exception cref="ArgumentException">If <paramref name="httpVersion"/> is invalid.</exception>
        public void Respond(string httpVersion, HttpStatusCode statusCode, string reason, string body, string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                contentType = "text/html";

            if (string.IsNullOrEmpty(httpVersion) || !httpVersion.StartsWith("HTTP/1"))
                throw new ArgumentException("Invalid HTTP version");

            if (string.IsNullOrEmpty(reason))
                reason = statusCode.ToString();

            string response = string.IsNullOrEmpty(body)
                                  ? httpVersion + " " + (int)statusCode + " " + reason + "\r\n\r\n"
                                  : string.Format("{0} {1} {2}\r\nContent-Type: {5}\r\nContent-Length: {3}\r\n\r\n{4}",
                                                  httpVersion, (int)statusCode, reason ?? statusCode.ToString(),
                                                  body.Length, body, contentType);
            byte[] buffer = Encoding.ASCII.GetBytes(response);

            Send(buffer);
            if (_currentRequest.Connection == ConnectionType.Close)
                FullRequestProcessed = true;

        }

        /// <summary>
        /// Send a response.
        /// </summary>
        /// <param name="httpVersion">Either <see cref="HttpHelper.HTTP10"/> or <see cref="HttpHelper.HTTP11"/></param>
        /// <param name="statusCode">HTTP status code</param>
        /// <param name="reason">reason for the status code.</param>
        public void Respond(string httpVersion, HttpStatusCode statusCode, string reason)
        {
            Respond(httpVersion, statusCode, reason, null, null);
        }

        /// <summary>
        /// Send a response.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public void Respond(string body)
        {
            if (body == null)
                throw new ArgumentNullException("body");
            Respond("HTTP/1.1", HttpStatusCode.OK, HttpStatusCode.OK.ToString(), body, null);
        }

        /// <summary>
        /// send a whole buffer
        /// </summary>
        /// <param name="buffer">buffer to send</param>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Send(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            return Send(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Send data using the stream
        /// </summary>
        /// <param name="buffer">Contains data to send</param>
        /// <param name="offset">Start position in buffer</param>
        /// <param name="size">number of bytes to send</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>

        private object sendLock = new object();

        public bool Send(byte[] buffer, int offset, int size)
        {
            // add some trivial checks required by opensim until another fix is possible
            // this are needed because opensim doesn't have access to stream state
            // and in its current state it will try to send to closed streams.
            if (Stream == null || _sock == null || !_sock.Connected)
                return false;

            lock (sendLock) // can't have overlaps here
            {
                bool ok = true;
                if (offset + size > buffer.Length)
                    throw new ArgumentOutOfRangeException("offset", offset, "offset + size is beyond end of buffer.");
                try
                {
                    // we are supposed to block so do block
                    if (_sock.Poll(30000000, SelectMode.SelectWrite) && Stream != null && _sock != null)
                        Stream.Write(buffer, offset, size);
                    else
                        ok = false;
                }
                //                catch(IOException e)
                catch
                {
                    // code to handle recoverable errors

                    //var socketExept = e.InnerException as SocketException;
                    //if (socketExept != null)
                    //{
                    //var errcode = socketExept.ErrorCode;
                    //}
                    ok = false;
                    //                    throw e; // let it still be visible
                }

                if (!ok && Stream != null)
                    Disconnect(SocketError.NoRecovery);

                return ok;
            }
        }

        /// <summary>
        /// The context have been disconnected.
        /// </summary>
        /// <remarks>
        /// Event can be used to clean up a context, or to reuse it.
        /// </remarks>
        public event EventHandler<DisconnectedEventArgs> Disconnected = delegate { };
        /// <summary>
        /// A request have been received in the context.
        /// </summary>
        public event EventHandler<RequestEventArgs> RequestReceived = delegate { };

        public HTTPNetworkContext GiveMeTheNetworkStreamIKnowWhatImDoing()
        {
            StreamPassedOff = true;
            _parser.RequestCompleted -= OnRequestCompleted;
            _parser.RequestLineReceived -= OnRequestLine;
            _parser.HeaderReceived -= OnHeaderReceived;
            _parser.BodyBytesReceived -= OnBodyBytesReceived;
            _parser.Clear();

            return new HTTPNetworkContext() { Socket = _sock, Stream = _stream as NetworkStream };
        }

        public void Dispose()
        {
            if (contextID >= 0)
            {
                StreamPassedOff = false;
                Cleanup();
            }
        }

        ~HttpClientContext()
        {
            Dispose();
        }
    }
}