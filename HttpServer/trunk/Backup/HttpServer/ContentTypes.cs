using System.Collections;

namespace HttpServer
{
    public class ContentType
    {
        public const string Text = "text/plain";
        public const string Html = "text/html";
        public const string Xml = "text/xml";
    }

    /// <summary>
    /// A list of content types
    /// </summary>
    public class ContentTypes : IEnumerable
    {
        private readonly string[] _contentTypes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types">Semicolon separated content types.</param>
        public ContentTypes(string types)
        {
            if (types == null)
                _contentTypes = new string[] { ContentType.Html };
            else
                _contentTypes = types.Split(';');
        }

        /// <summary>
        /// Get this first content type.
        /// </summary>
        public string First
        {
            get { return _contentTypes.Length == 0 ? string.Empty : _contentTypes[0]; }
        }

        /// <summary>
        /// Fetch a content type
        /// </summary>
        /// <param name="type">Part of type ("xml" would return "application/xml")</param>
        /// <returns></returns>
        public string this[string type]
        {
            get
            {
                foreach (string contentType in _contentTypes)
                {
                    if (contentType.Contains(type))
                        return contentType;
                }

                return string.Empty;
            }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _contentTypes.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Searches for the specified type
        /// </summary>
        /// <param name="type">Can also be a part of a type (searching for "xml" would return true for "application/xml").</param>
        /// <returns>true if type was found.</returns>
        public bool Contains(string type)
        {
            foreach (string contentType in _contentTypes)
            {
                if (contentType.Contains(type))
                    return true;
            }

            return false;
        }
    }
}