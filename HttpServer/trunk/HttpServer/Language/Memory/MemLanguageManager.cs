using System.Collections.Generic;

namespace HttpServer.Language.Memory
{
    /// <summary>
    /// Language manager that keeps everything in memory.
    /// </summary>
    /// <remarks>Default language us 1033 (en-us).</remarks>
    public class MemLanguageManager : LanguageManager
    {
        private readonly Dictionary<string, LanguageCategory> _categories = new Dictionary<string, LanguageCategory>();
        private int _defaultLCID = 1033;

        public MemLanguageManager()
        {
            _categories.Add(LanguageCategory.Default, new MemLanguageCategory(_defaultLCID, LanguageCategory.Default));
        }
        #region LanguageManager Members

        /// <summary>
        /// LCID to use if requested lcid is not found.
        /// </summary>
        public int DefaultLcid
        {
            get { return _defaultLCID; }
            set
            {
                _defaultLCID = value;
                lock (_categories)
                    foreach (KeyValuePair<string, LanguageCategory> pair in _categories)
                        pair.Value.SetDefaultLCID(_defaultLCID);
            }
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
        public void Add(string textName, int lcid, string phrase)
        {
            Add(textName, lcid, LanguageCategory.Default, phrase);
        }

        public void Add(string textName, int lcid, string category, string phrase)
        {
            lock (_categories)
            {
                if (!_categories.ContainsKey(category))
                    _categories.Add(category,
                                    new MemLanguageCategory(_defaultLCID, category));

                _categories[category].Add(textName, lcid, phrase);
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
            get { return this[LanguageCategory.Default, textName]; }
        }

        public string this[string textName, string category]
        {
            get
            {
                lock (_categories)
                {
                    if (_categories.ContainsKey(category))
                        return _categories[category][textName];
                    else
                        return LanguageCategory.EmptyValue(textName);
                }
            }
        }

        public string this[string textName, string category, int lcid]
        {
            get 
            {
                lock (_categories)
                {
                    if (_categories.ContainsKey(category))
                        return _categories[category][textName, lcid];
                    else
                        return LanguageCategory.EmptyValue(textName);
                }
            }
        }

        public LanguageCategory GetCategory(string name)
        {
            lock (_categories)
            {
                if (_categories.ContainsKey(name))
                    return _categories[name];
                else
                    return LanguageCategory.Empty;
            }
        }

        /// <summary>
        /// Number of languages.
        /// </summary>
        public int Count
        {
            get { return _categories.Count; }
        }

        #endregion
    }
}