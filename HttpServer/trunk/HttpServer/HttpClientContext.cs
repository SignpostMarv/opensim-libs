using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HttpServer.Exceptions;

namespace HttpServer
{
    public class HttpClientContext
    {
        /// <summary>
        /// Buffersize determines how large the HTTP header can be.
        /// </summary>
        public static int BufferSize = 16384;
        private readonly byte[] _buffer = new byte[BufferSize];
        private int _bytesLeft;
        private readonly HttpRequest _currentRequest = new HttpRequest();
        private readonly ClientDisconnectedHandler _disconnectHandler;
        private readonly WriteLogHandler _log;
        private readonly HttpRequestParser _parser;
        private readonly RequestReceivedHandler _requestHandler;
        private readonly bool _secured;
        private readonly Stream _stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secured">true if the connection is secured (SSL/TLS)</param>
        /// <param name="handler">delegate handling incoming requests.</param>
        /// <param name="disconnectHandler">delegate being called when a client disconnectes</param>
        /// <param name="stream">Stream used for communication</param>
        /// <exception cref="SocketException">If beginreceive fails</exception>
        /// <param name="writer">delegate used to write log entries</param>
        /// <see cref="RequestReceivedHandler"/>
        /// <see cref="ClientDisconnectedHandler"/>
        public HttpClientContext(bool secured, RequestReceivedHandler handler,
                                   ClientDisconnectedHandler disconnectHandler,
                                   Stream stream, WriteLogHandler writer)
            : this(secured, stream, handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _log = writer;
            _disconnectHandler = disconnectHandler;
        }

        public HttpClientContext(bool secured, Stream stream, RequestReceivedHandler requestHandler)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanWrite || !stream.CanRead)
                throw new ArgumentException("Stream must be writeable and readable..");
            if (requestHandler == null)
                throw new ArgumentNullException("requestHandler");

            _parser = new HttpRequestParser(OnRequestCompleted, _log);
            _secured = secured;
            _requestHandler = requestHandler;
            _disconnectHandler = null;
            _log = null;
            _stream = stream;
            _stream.BeginRead(_buffer, 0, BufferSize, OnReceive, null);
        }

        /// <summary>
        /// Using SSL or other encryption method.
        /// </summary>
        public bool Secured
        {
            get { return _secured; }
        }

        internal void Disconnect(SocketError error)
        {
            _stream.Close();
            if (_disconnectHandler != null)
                _disconnectHandler(this, error);
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                int bytesRead = _stream.EndRead(ar);
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
                WriteLog(this, LogPrio.Trace, "Received: " + temp);
#pragma warning restore 219
#endif
                int offset = _parser.ParseMessage(_buffer, 0, _bytesLeft);

                // try again to see if we can parse another message (check parser to see if it is looking for a new message)
                int oldOffset = offset;
                while (_parser.CurrentState == HttpRequestParser.State.FirstLine && offset != 0 && _bytesLeft - offset > 0)
                {
#if DEBUG
                    temp = Encoding.ASCII.GetString(_buffer, offset, _bytesLeft - offset);
                    WriteLog(this, LogPrio.Trace, "Processing: " + temp);
#endif
                    offset = _parser.ParseMessage(_buffer, offset, _bytesLeft - offset);
                }
                // need to be able to move prev bytes, so restore offset.
                if (offset == 0)
                    offset = oldOffset;

                // copy unused bytes to the beginning of the array
                if (offset > 0 && _bytesLeft != offset)
                {
                    int bytesToMove = _bytesLeft - offset;
                    for (int i = 0; i < bytesToMove; ++i)
                        _buffer[i] = _buffer[i + offset];
                }

                _bytesLeft -= offset;
                _stream.BeginRead(_buffer, _bytesLeft, _buffer.Length - _bytesLeft, OnReceive, null);
            }
            catch (BadRequestException err)
            {
                WriteLog(this, LogPrio.Warning, err.Message);
                
                if (string.IsNullOrEmpty(_currentRequest.HttpVersion))
                    Respond("HTTP/1.0", HttpStatusCode.BadRequest, err.Message);
                else
                    Respond(_currentRequest.HttpVersion, HttpStatusCode.BadRequest, err.Message);
                
                Disconnect(SocketError.NoRecovery);
                throw;
            }
            catch (IOException err)
            {
                WriteLog(this, LogPrio.Info, "Failed to end receive: " + err.Message);
                if (err.InnerException is SocketException)
                    Disconnect((SocketError) ((SocketException) err.InnerException).ErrorCode);
                else
                    Disconnect(SocketError.ConnectionReset);
            }
            catch (ObjectDisposedException err)
            {
                WriteLog(this, LogPrio.Warning, "Failed to end receive : " + err.Message);
                Disconnect(SocketError.NotSocket);
            }
        }

        private void OnRequestCompleted(HttpRequest request)
        {
            if (_requestHandler != null)
                _requestHandler(this, request);
        }

        /// <summary>
        /// Send a response.
        /// </summary>
        /// <param name="httpVersion">Either HttpHelper.HTTP10 or HttpHelper.HTTP11</param>
        /// <param name="statusCode">http status code</param>
        /// <param name="reason">reason for the status code.</param>
        /// <param name="body">html body contents, can be null or empty.</param>
        /// <exception cref="ArgumentException">If httpVersion is invalid.</exception>
        public void Respond(string httpVersion, HttpStatusCode statusCode, string reason, string body)
        {
            if (string.IsNullOrEmpty(httpVersion) || !httpVersion.StartsWith("HTTP/1"))
                throw new ArgumentException("Invalid HTTP version");
            
            byte[] buffer;
            if (string.IsNullOrEmpty(body))
                buffer = Encoding.ASCII.GetBytes(httpVersion + " " + (int) statusCode + " " + reason + "\r\n\r\n");
            else
            {
                buffer =
                    Encoding.ASCII.GetBytes(
                        string.Format("{0} {1} {2}\r\nContent-Type: text/html\r\nContent-Length: {3}\r\n\r\n{4}",
                                      httpVersion, (int) statusCode, reason ?? statusCode.ToString(), body.Length, body));
            }

            Send(buffer);
        }

        /// <summary>
        /// Send a response.
        /// </summary>
        /// <param name="httpVersion">Either HttpHelper.HTTP10 or HttpHelper.HTTP11</param>
        /// <param name="statusCode">http status code</param>
        /// <param name="reason">reason for the status code.</param>
        public void Respond(string httpVersion, HttpStatusCode statusCode, string reason)
        {
            Respond(httpVersion, statusCode, reason, null);
        }

        /// <summary>
        /// Send a response.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public void Respond(string body)
        {
            if (body == null) 
                throw new ArgumentNullException("body");
            Respond("HTTP/1.1", HttpStatusCode.OK, HttpStatusCode.OK.ToString(), body);
        }

        public void WriteLog(object source, LogPrio prio, string entry)
        {
            if (_log == null)
                return;
            _log(source, prio, entry);
        }
        /// <summary>
        /// send a whole buffer
        /// </summary>
        /// <param name="buffer">buffer to send</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Send(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            Send(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Send data using the stream
        /// </summary>
        /// <param name="buffer">Contains data to send</param>
        /// <param name="offset">Start position in buffer</param>
        /// <param name="size">number of bytes to send</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Send(byte[] buffer, int offset, int size)
        {
            if (offset + size > buffer.Length)
                throw new ArgumentOutOfRangeException("offset", offset, "offset + size is beyond end of buffer.");

            _stream.Write(buffer, offset, size);
        }

    }


    /// <summary>
    /// Client have been disconnected.
    /// </summary>
    /// <param name="client">Client that was disconnected.</param>
    /// <param name="error">Reason</param>
    /// <see cref="HttpClientContext"/>
    public delegate void ClientDisconnectedHandler(HttpClientContext client, SocketError error);

    /// <summary>
    /// Invoked when a client context have received a new HTTP request
    /// </summary>
    /// <param name="client">Client that received the request.</param>
    /// <param name="request">Request that was received.</param>
    /// <see cref="HttpClientContext"/>
    public delegate void RequestReceivedHandler(HttpClientContext client, HttpRequest request);
}