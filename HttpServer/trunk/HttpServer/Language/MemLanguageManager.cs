using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;

namespace HttpServer.Language
{
    /// <summary>
    /// Language manager that keeps everything in memory.
    /// </summary>
    /// <remarks>Default language us 1033 (en-us).</remarks>
    public class MemLanguageManager : LanguageManager
    {
        /// <summary>
        /// int is LCID, NameValueCollection contains all phrases.
        /// </summary>
        private readonly Dictionary<int, NameValueCollection> _languages = new Dictionary<int, NameValueCollection>();

        private int _defaultLCID = 1033;

        /// <summary>
        /// LCID to use if requested lcid is not found.
        /// </summary>
        public int DefaultLcid
        {
            get { return _defaultLCID; }
            set { _defaultLCID = value; }
        }

        /// <summary>
        /// Add a text to a language.
        /// </summary>
        /// <param name="lcid">Language that the text should be added to</param>
        /// <param name="textName">Name used to identify the text.</param>
        /// <param name="phrase">Text to add</param>
        /// <example>
        /// langMgr.Add(1053, "ValidatorRequired", "{0} is required.");
        /// </example>
        public void Add(int lcid, string textName, string phrase)
        {
            lock (_languages)
            {
                if (!_languages.ContainsKey(lcid))
                    _languages.Add(lcid, new NameValueCollection());

                _languages[lcid].Add(textName, phrase);
            }
        }

        /// <summary>
        /// Get a language
        /// </summary>
        /// <param name="lcid">language to get</param>
        /// <returns>Collection of texts if found; otherwise null.</returns>
        public NameValueCollection this[int lcid]
    {
        get
        {
            lock (_languages)
            {
                if (_languages.ContainsKey(lcid))
                    return _languages[lcid];
                else
                    return null;
            }
        }
    }

        /// <summary>
        /// Get a phrase for the current language.
        /// </summary>
        /// <param name="textName">Phrase to find.</param>
        /// <returns>text if found; string.Empty if not.</returns>
        /// <remarks>Uses .CurrentThread.CurrentCulture.LCID to determine the current language. Will use DefaultLcid if current language is not found.</remarks>
        public string this[string textName]
        {
            get
            {
                int lcId = Thread.CurrentThread.CurrentCulture.LCID;
                lock (_languages)
                {
                    if (!_languages.ContainsKey(lcId))
                    {
                        if (!_languages.ContainsKey(DefaultLcid))
                            return string.Empty;

                        return _languages[DefaultLcid][textName] ?? string.Empty;
                    }

                    return _languages[lcId][textName] ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Find a phrase in a specific language
        /// </summary>
        /// <param name="lcid">language to look in.</param>
        /// <param name="textName">phrase to find.</param>
        /// <returns>Text if found; otherwise string.Empty.</returns>
        public string GetPhrase(int lcid, string textName)
        {
            lock (_languages)
            {
                if (!_languages.ContainsKey(lcid))
                    return string.Empty;

                return _languages[lcid][textName] ?? string.Empty;
            }
        }

        #region LanguageManager Members

        /// <summary>
        /// Number of languages.
        /// </summary>
        public int Count
        {
            get { return _languages.Count; }
        }

        #endregion
    }

}
