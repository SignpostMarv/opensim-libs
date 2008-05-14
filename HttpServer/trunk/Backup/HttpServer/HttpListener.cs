using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace HttpServer
{
    /// <summary>
    /// HTTP Listener waits for HTTP connections and provide us with HttpListenerContexts using the
    /// <see cref="RequestHandler"/> delegate.
    /// </summary>
    public class HttpListener
    {
        private readonly IPAddress _address;
        private readonly X509Certificate _certificate;
        private readonly int _port;
        private readonly SslProtocols _sslProtocol = SslProtocols.Tls;
        private ClientDisconnectedHandler _disconnectHandler;
        private TcpListener _listener;
        private WriteLogHandler _logWriter;
        private RequestReceivedHandler _requestHandler;
        private readonly object _logLock = new object();

        /// <summary>
        /// Listen for regular http connections
        /// </summary>
        /// <param name="address">IP Address to accept connections on</param>
        /// <param name="port">TCP Port to listen on, default HTTP port is 80.</param>
        public HttpListener(IPAddress address, int port)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            if (port <= 0)
                throw new ArgumentException("Port must be a positive number.");

            _address = address;
            _port = port;
        }

        /// <summary>
        /// Launch HttpListener in SSL mode
        /// </summary>
        /// <param name="address">IP Address to accept connections on</param>
        /// <param name="port">TCP Port to listen on, default HTTPS port is 443</param>
        /// <param name="certificate">Certificate to use</param>
        public HttpListener(IPAddress address, int port, X509Certificate certificate) : this(address, port)
        {
            _certificate = certificate;
        }

        /// <summary>
        /// Launch HttpListener in SSL mode
        /// </summary>
        /// <param name="address">IP Address to accept connections on</param>
        /// <param name="port">TCP Port to listen on, default HTTPS port is 443</param>
        /// <param name="certificate">Certificate to use</param>
        /// <param name="protocol">which https protocol to use, default is Tls.</param>
        public HttpListener(IPAddress address, int port, X509Certificate certificate, SslProtocols protocol)
            : this(address, port, certificate)
        {
            _sslProtocol = protocol;
        }

        /// <summary>
        /// Invoked when a client disconnects
        /// </summary>
        public ClientDisconnectedHandler DisconnectHandler
        {
            get { return _disconnectHandler; }
            set { _disconnectHandler = value; }
        }

        /// <summary>
        /// Gives you a change to receive log entries for all internals of the http lib.
        /// </summary>
        public WriteLogHandler LogWriter
        {
            get { return _logWriter; }
            set
            {
                lock (_logLock)
                {
                    _logWriter = value;
                    if (_certificate != null)
                        WriteLog(this, LogPrio.Info,
                                 "HTTPS(" + _sslProtocol + ") listening on " + _address + ":" + _port);
                    else
                        WriteLog(this, LogPrio.Info, "HTTP listening on " + _address + ":" + _port);
                }
            }
        }

        /// <summary>
        /// This handler will be invoked each time a new connection is accepted.
        /// </summary>
        public RequestReceivedHandler RequestHandler
        {
            get { return _requestHandler; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _requestHandler = value;
            }
        }

        protected IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state)
        {
            return _listener.BeginAcceptSocket(callback, state);
        }

        protected HttpClientContext EndAcceptSocket(IAsyncResult ar)
        {
            // if we have stopped the listener.
            if (_listener == null)
                return null;
            if (_requestHandler == null)
                return null;

            Socket socket = _listener.EndAcceptSocket(ar);
            WriteLog(this, LogPrio.Debug, "Accepted connection from: " + socket.RemoteEndPoint);
            NetworkStream stream = new NetworkStream(socket, true);
            if (_certificate != null)
            {
                SslStream sslStream = new SslStream(stream, false);
                sslStream.AuthenticateAsServer(_certificate, false, _sslProtocol, true);
                return new HttpClientContext(true, _requestHandler, _disconnectHandler, sslStream, LogWriter);
            }
            else
                return new HttpClientContext(false, _requestHandler, _disconnectHandler, stream, LogWriter);
        }

        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                // Let's end the request.
                EndAcceptSocket(ar);
                BeginAcceptSocket(OnAccept, null);
            }
            catch (Exception err)
            {
                if (ExceptionThrown == null)
#if DEBUG
                    throw;
#else
    // we can't really do anything but close the connection
                    Console.WriteLine(err.Message);
#endif
                else
                    ExceptionThrown(this, err);
            }
            finally
            {
                try
                {
                    // we do this in finally since we REALLY want the http listener to keep
                    // running.
                    // but don't do it if we fail
                    BeginAcceptSocket(OnAccept, null);
                }
                catch (Exception err)
                {
                    if (ExceptionThrown == null)
#if DEBUG
                        throw;
#else
    // we can't really do anything but close the connection
                    Console.WriteLine(err.Message);
#endif
                    else
                        ExceptionThrown(this, err);
                }
            }
        }

        /// <summary>
        /// Start listen for new connections
        /// </summary>
        /// <param name="backlog">Number of connections that can stand in a queue to be accepted.</param>
        public void Start(int backlog)
        {
            if (_listener == null)
            {
                _listener = new TcpListener(_address, _port);
                _listener.Start(backlog);
            }

            BeginAcceptSocket(OnAccept, null);
        }


        /// <summary>
        /// Stop the listener
        /// </summary>
        /// <exception cref="SocketException"></exception>
        public void Stop()
        {
            _listener.Stop();
            _listener = null;
        }

        public void WriteLog(object source, LogPrio prio, string message)
        {
            lock (_logLock)
            {
                if (_logWriter != null)
                    _logWriter(source, prio, message);
            }
        }

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