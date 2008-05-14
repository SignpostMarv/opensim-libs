using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using HttpServer.Exceptions;
using HttpServer.FormDecoders;
using HttpServer;

namespace HttpServer
{
    /// <summary>
    /// Contains serverside http request information.
    /// </summary>
    public class HttpRequest : ICloneable
    {
        public static readonly char[] UriSplitters = new char[] {'/'};
        private readonly NameValueCollection _headers = new NameValueCollection();
        private readonly bool _secure = false;
        private string[] _acceptTypes = null;
        private Stream _body = new MemoryStream();
        private int _bodyBytesLeft;
        private ConnectionType _connection = ConnectionType.Close;
        private int _contentLength;
        private string _httpVersion = string.Empty;
        private string _method = string.Empty;
        private HttpInput _queryString = HttpInput.Empty;
        private Uri _uri = HttpHelper.EmptyUri;
        private string[] _uriParts;
        private string _uriPath;
        private HttpInput _form = HttpInput.Empty;
        private readonly HttpParam _param = new HttpParam(HttpInput.Empty, HttpInput.Empty);
        private bool _isAjax;
        private RequestCookies _cookies;

        public bool BodyIsComplete
        {
            get
            {
                return _bodyBytesLeft == 0;
            }
        }
        /// <summary>
        /// Kind of types accepted by the client.
        /// </summary>
        public string[] AcceptTypes
        {
            get { return _acceptTypes; }
        }

        /// <summary>
        /// Submitted body contents
        /// </summary>
        public Stream Body
        {
            get { return _body; }
            set { _body = value; }
        }

        /// <summary>
        /// Kind of connection used for the session.
        /// </summary>
        public ConnectionType Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        /// <summary>
        /// Number of bytes in the body
        /// </summary>
        public int ContentLength
        {
            get { return _contentLength; }
            set
            {
                _contentLength = value;
                _bodyBytesLeft = value;
            }
        }

        /// <summary>
        /// Decode body into a form.
        /// </summary>
        /// <param name="providers">A list with form decoders.</param>
        /// <exception cref="InvalidDataException">If body contents is not valid for the chosen decoder.</exception>
        /// <exception cref="InvalidOperationException">If body is still being transferred.</exception>
        public void DecodeBody(FormDecoderProvider providers)
        {
            if (_bodyBytesLeft > 0)
                throw new InvalidOperationException("Body have not yet been completed.");

            _form = providers.Decode(_headers["content-type"], _body, Encoding.UTF8);
            if (_form != HttpInput.Empty)
                _param.SetForm(_form);
        }

        /// <summary>
        /// Headers sent by the client. All names are in lower case.
        /// </summary>
        public NameValueCollection Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// Version of http. 
        /// Probably HttpHelper.HTTP10 or HttpHelper.HTTP11
        /// </summary>
        /// <seealso cref="HttpHelper"/>
        public string HttpVersion
        {
            get { return _httpVersion; }
            set { _httpVersion = value; }
        }

        /// <summary>
        /// Requested method, always upper case.
        /// </summary>
        /// <remarks>
        /// The methods GET and HEAD MUST be supported by all general-purpose servers. All other methods are OPTIONAL
        /// <para>
        /// The OPTIONS method represents a request for information about the communication options available 
        /// on the request/response chain identified by the Request-URI. This method allows the client to determine 
        /// the options and/or requirements associated with a resource, or the capabilities of a server, without implying 
        /// a resource action or initiating a resource retrieval.
        /// </para><para>
        /// The GET method means retrieve whatever information (in the form of an entity) is identified by the Request-URI.
        ///  If the Request-URI refers to a data-producing process, it is the produced data which shall be returned 
        /// as the entity in the response and not the source text of the process, unless that text happens to be the
        ///  output of the process.
        /// </para><para>
        /// The HEAD method is identical to GET except that the server MUST NOT return a message-body in the response. 
        /// The metainformation contained in the HTTP headers in response to a HEAD request SHOULD be identical to 
        /// the information sent in response to a GET request. This method can be used for obtaining metainformation 
        /// about the entity implied by the request without transferring the entity-body itself. This method is often used 
        /// for testing hypertext links for validity, accessibility, and recent modification.
        /// </para> <para>
        /// The POST method is used to request that the origin server accept the entity enclosed in the request as a new 
        /// subordinate of the resource identified by the Request-URI in the Request-Line.
        ///  The action performed by the POST method might not result in a resource that can be identified by a URI. In this case, 
        /// either 200 (OK) or 204 (No Content) is the appropriate response status, depending on whether or not the response
        ///  includes an entity that describes the result.
        ///   If a resource has been created on the origin server, the response SHOULD be 201 (Created) and contain an entity 
        /// which describes the status of the request and refers to the new resource, and a Location header
        /// </para>
        /// <para>
        /// The PUT method requests that the enclosed entity be stored under the supplied Request-URI. 
        /// If the Request-URI refers to an already existing resource, the enclosed entity SHOULD be considered as a modified 
        /// version of the one residing on the origin server. If the Request-URI does not point to an existing resource, 
        /// and that URI is capable of being defined as a new resource by the requesting user agent, the origin server can 
        /// create the resource with that URI. If a new resource is created, the origin server MUST inform the user agent via 
        /// the 201 (Created) response. If an existing resource is modified, either the 200 (OK) or 204 (No Content) response 
        /// codes SHOULD be sent to indicate successful completion of the request. 
        /// 
        /// If the resource could not be created or modified with the Request-URI, an appropriate error response SHOULD be 
        /// given that reflects the nature of the problem. The recipient of the entity MUST NOT ignore any Content-* (e.g. Content-Range) 
        /// headers that it does not understand or implement and MUST return a 501 (Not Implemented) response in such cases.
        /// </para>
        /// <para>
        /// The DELETE method requests that the origin server delete the resource identified by the Request-URI. 
        /// This method MAY be overridden by human intervention (or other means) on the origin server. 
        /// The client cannot be guaranteed that the operation has been carried out, even if the status code returned 
        /// from the origin server indicates that the action has been completed successfully. However, the server SHOULD NOT 
        /// indicate success unless, at the time the response is given, it intends to delete the resource or move it to an inaccessible location.
        /// </para>
        /// </remarks>
        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }

        /// <summary>
        /// Variables sent in the query string
        /// </summary>
        public HttpInput QueryString
        {
            get { return _queryString; }
        }

        /// <summary>
        /// Requested URI (url)
        /// </summary>
        /// <seealso cref="UriPath"/>
        public Uri Uri
        {
            get { return _uri; }
            set
            {
                if (value == null)
                    _uri = HttpHelper.EmptyUri;
                else
                    _uri = value;

                _uri = value;
                _uriParts = _uri.AbsolutePath.Split(UriSplitters, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Uri absolute path splitted into parts.
        /// </summary>
        /// <example>
        /// // uri is: http://gauffin.com/code/tiny/
        /// Console.WriteLine(request.UriParts[0]); // result: code
        /// Console.WriteLine(request.UriParts[1]); // result: tiny
        /// </example>
        /// <remarks>
        /// If you're using controllers than the first part is controller name,
        /// the second part is method name and the third part is Id property.
        /// </remarks>
        /// <seealso cref="Uri"/>
        public string[] UriParts
        {
            get { return _uriParts; }
        }

        /// <summary>
        /// Path and query (will be merged with the host header) and put in Uri
        /// </summary>
        /// <see cref="Uri"/>
        internal string UriPath
        {
            get { return _uriPath; }
            set
            {
                _uriPath = value;
                int pos = _uriPath.IndexOf('?');
                if (pos != -1)
                {
                    _queryString = HttpHelper.ParseQueryString(_uriPath.Substring(pos + 1));
                    _param.SetQueryString(_queryString);
                    _uriParts = value.Substring(0, pos).Split(UriSplitters, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                    _uriParts = value.Split(UriSplitters, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Check's both QueryString and Form after the parameter.
        /// </summary>
        public HttpParam Param
        {
            get { return _param; }
        }

        /// <summary>
        /// Form parameters.
        /// </summary>
        public HttpInput Form
        {
            get { return _form; }
        }

        public bool IsAjax
        {
            get { return _isAjax; }
        }

        public RequestCookies Cookies
        {
            get { return _cookies; }
        }

        internal void SetCookies(RequestCookies cookies)
        {
            _cookies = cookies;
        }

        /// <summary>
        /// Called during parsing of a HttpRequest.
        /// </summary>
        /// <param name="name">Name of the header, should not be url encoded</param>
        /// <param name="value">Value of the header, should not be url encoded</param>
        /// <exception cref="BadRequestException">If a header is incorrect.</exception>
        public void AddHeader(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new BadRequestException("Invalid header name: " + name ?? "<null>");
            if (string.IsNullOrEmpty(value))
                throw new BadRequestException("Header '" + name + "' do not contain a value.");

            name = name.ToLower();
            switch (name)
            {
                    
                case "http_x_requested_with":
                case "x-requested-with":
                    if (string.Compare(value, "XMLHttpRequest", true) == 0)
                        _isAjax = true;
                    break;
                case "accept":
                    _acceptTypes = value.Split(',');
                    for (int i = 0; i < _acceptTypes.Length; ++i )
                        _acceptTypes[i] = _acceptTypes[i].Trim();
                    break;
                case "content-length":
                    int t;
                    if (!int.TryParse(value, out t))
                        throw new BadRequestException("Invalid content length.");
                    ContentLength = t;
                    break; //todo: mayby throw an exception
                case "host":
                    try
                    {
                        _uri = new Uri(_secure ? "https://" : "http://" + value + _uriPath);
                        _uriParts = _uri.AbsolutePath.Split(UriSplitters, StringSplitOptions.RemoveEmptyEntries);
                    }
                    catch (UriFormatException err)
                    {
                        throw new BadRequestException("Failed to parse uri: " + value + _uriPath, err);
                    }
                    break;
                case "connection":
                    if (string.Compare(value, "close", true) == 0)
                        Connection = ConnectionType.Close;
                    else if (string.Compare(value, "keep-alive", true) == 0)
                        Connection = ConnectionType.KeepAlive;
                    else
                        throw new BadRequestException("Unknown 'Connection' header type.");
                    break;
            }
            // Some of the headers are being added to times. Maybe we should not do that in the future?
            _headers.Add(name, value);
        }

        /// <summary>
        /// Add bytes to the body
        /// </summary>
        /// <param name="bytes">buffer to read bytes from</param>
        /// <param name="offset">where to start read</param>
        /// <param name="length">number of bytes to read</param>
        /// <returns>Number of bytes actually read (same as length unless we got all body bytes).</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException">If body is not writable</exception>
        internal int AddToBody(byte[] bytes, int offset, int length)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");
            if (offset + length  > bytes.Length)
                throw new ArgumentOutOfRangeException("offset");
            if (length == 0)
                return 0;
            if (!_body.CanWrite)
                throw new InvalidOperationException("Body is not writable.");

            if (length > _bodyBytesLeft)
            {
                length = _bodyBytesLeft;
                
            }

            _body.Write(bytes, offset, length);
            _bodyBytesLeft -= length;

            return length;
        }

        /// <summary>
        /// Clear everything in the request
        /// </summary>
        internal void Clear()
        {
            _body.Seek(0, SeekOrigin.Begin);
            _contentLength = 0;
            _method = string.Empty;
            _uri = HttpHelper.EmptyUri;
            _queryString = HttpInput.Empty;
            _bodyBytesLeft = 0;
            _headers.Clear();
            _connection = ConnectionType.Close;
        }

        #region ICloneable Members

        ///<summary>
        ///Creates a new object that is a copy of the current instance.
        ///</summary>
        ///
        ///<returns>
        ///A new object that is a copy of this instance.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}