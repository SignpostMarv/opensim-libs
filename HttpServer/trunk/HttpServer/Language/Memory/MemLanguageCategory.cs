using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;

namespace HttpServer.Language.Memory
{
    public class MemLanguageCategory : LanguageCategory
    {
        /// <summary>
        /// int is LCID, NameValueCollection contains all phrases.
        /// </summary>
        private readonly Dictionary<int, NameValueCollection> _languages = new Dictionary<int, NameValueCollection>();
        private readonly string _name;

        public MemLanguageCategory(int defaultLCID, string name) : base(defaultLCID)
        {
            _name = name;
        }

        public override string this[string textName]
        {
            get
            {
                int lcId = Thread.CurrentThread.CurrentCulture.LCID;
                return this[textName, lcId];
            }
        }

        public override string this[string textName, int lcid]
        {
            get {
                lock (_languages)
                {
                    if (!_languages.ContainsKey(lcid) && _languages.ContainsKey(DefaultLCID))
                        return _languages[DefaultLCID][textName] ?? string.Format("[{0}]", textName);

                    return _languages[lcid][textName] ?? string.Format("[{0}]", textName);
                }
            }
        }

        public override int Count
        {
            get { return _languages.Count; }
        }

        public string Name
        {
            get { return _name; }
        }

        public override void Add(string name, int lcid, string text)
        {
            lock (_languages)
            {
                if (!_languages.ContainsKey(lcid))
                    _languages.Add(lcid, new NameValueCollection());

                _languages[lcid].Add(name, text);
            }

        }

        public override int GetTextCount(int lcid)
        {
            lock (_languages)
            {
                if (_languages.ContainsKey(lcid))
                    return _languages[lcid].Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Determine if a category contains a specific language.
        /// </summary>
        /// <param name="lcid"></param>
        /// <returns></returns>
        public override bool Contains(int lcid)
        {
            lock (_languages)
                return _languages.ContainsKey(lcid);
        }
    }
}