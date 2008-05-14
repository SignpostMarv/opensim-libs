namespace HttpServer.Language
{
    public abstract class LanguageCategory
    {
        public static EmptyCategory Empty = new EmptyCategory(0);

        public static string Default = "___";
        private int _defaultLCID;

        public LanguageCategory(int defaultLCID)
        {
            _defaultLCID = defaultLCID;
        }


        /// <summary>
        /// Add a localized text string.
        /// </summary>
        /// <param name="lcid">locale</param>
        /// <param name="name">Name identifying the string. Used to fetch the string later on.</param>
        /// <param name="text">Localized string</param>
        /// <remarks>String will be added to the default category.</remarks>
        /// <example>
        /// lang.Add("Name", "Name");
        /// </example>
        public abstract void Add(string name, int lcid, string text);

        /// <summary>
        /// Get a localized text string in the current laguage.
        /// </summary>
        /// <param name="textName">Phrase to find.</param>
        /// <returns>text if found; [textName] if not.</returns>
        /// <example>
        /// lang["Name"] // => "Name"
        /// lang["Naem"] // => "[Naem]" since it's missing
        /// </example>
        public abstract string this[string textName] { get; }

        /// <summary>
        /// Get a localized text string
        /// </summary>
        /// <param name="lcid"></param>
        /// <param name="textName">Phrase localeto find.</param>
        /// <returns>text if found; [textName] if not.</returns>
        /// <example>
        /// lang["Name"] // => "Name"
        /// lang["Naem"] // => "[Naem]" since it's missing
        /// </example>
        public abstract string this[string textName, int lcid] { get; }

        /// <summary>
        /// Number languages
        /// </summary>
        public abstract int Count
        { get;}

        /// <summary>
        /// LCID to use if the specified or current lcid is not found.
        /// </summary>
        public int DefaultLCID
        {
            get { return _defaultLCID; }
        }

        /// <summary>
        /// Number of translated texts in the specified language
        /// </summary>
        /// <param name="lcid"></param>
        /// <returns></returns>
        public abstract int GetTextCount(int lcid);

        /// <summary>
        /// Value that should be returned if the text is not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string EmptyValue(string name)
        {
            return string.Format("[{0}]", name);
        }

        internal void SetDefaultLCID(int lcid)
        {
            _defaultLCID = lcid;
        }

        /// <summary>
        /// Determine if a category contains a specific language.
        /// </summary>
        /// <param name="lcid"></param>
        /// <returns></returns>
        public abstract bool Contains(int lcid);
    }


    public class EmptyCategory : LanguageCategory
    {
        public EmptyCategory(int defaultLCID)
            : base(defaultLCID)
        {
        }

        /// <summary>
        /// Add a localized text string.
        /// </summary>
        /// <param name="lcid">locale</param>
        /// <param name="name">Name identifying the string. Used to fetch the string later on.</param>
        /// <param name="text">Localized string</param>
        /// <remarks>String will be added to the default category.</remarks>
        /// <example>
        /// lang.Add("Name", "Name");
        /// </example>
        public override void Add(string name, int lcid, string text)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get a localized text string in the current laguage.
        /// </summary>
        /// <param name="textName">Phrase to find.</param>
        /// <returns>text if found; [textName] if not.</returns>
        /// <example>
        /// lang["Name"] // => "Name"
        /// lang["Naem"] // => "[Naem]" since it's missing
        /// </example>
        public override string this[string textName]
        {
            get { return EmptyValue(textName); }
        }

        /// <summary>
        /// Get a localized text string
        /// </summary>
        /// <param name="lcid"></param>
        /// <param name="textName">Phrase localeto find.</param>
        /// <returns>text if found; [textName] if not.</returns>
        /// <example>
        /// lang["Name"] // => "Name"
        /// lang["Naem"] // => "[Naem]" since it's missing
        /// </example>
        public override string this[string textName, int lcid]
        {
            get { return EmptyValue(textName); }
        }

        /// <summary>
        /// Number languages
        /// </summary>
        public override int Count
        {
            get { return 0; }
        }

        /// <summary>
        /// Number of translated texts in the specified language
        /// </summary>
        /// <param name="lcid"></param>
        /// <returns></returns>
        public override int GetTextCount(int lcid)
        {
            return 0;
        }

        /// <summary>
        /// Determine if a category contains a specific language.
        /// </summary>
        /// <param name="lcid"></param>
        /// <returns></returns>
        public override bool Contains(int lcid)
        {
            return false;
        }
    }
}
