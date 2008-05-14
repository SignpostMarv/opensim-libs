namespace HttpServer.Language
{
    /// <summary>
    /// A language manager is used to take care of providing localized texts.
    /// </summary>
    /// <remarks>
    /// The manager can serve multiple languages and each language can either
    /// contain a flat structure or be divided into multiple categories.
    /// </remarks>
    public interface LanguageManager
    {
        /// <summary>
        /// LCID to use if requested lcid is not found.
        /// </summary>
        int DefaultLcid { get; set; }

        /// <summary>
        /// Add a text into the default category of the chosen language.
        /// </summary>
        /// <param name="lcid">Language that the text should be added to</param>
        /// <param name="textName">Name used to identify the text.</param>
        /// <param name="phrase">Text to add</param>
        /// <example>
        /// langMgr.Add(1053, "Required", "{0} is required.");
        /// </example>
        void Add(string textName, int lcid, string phrase);

        /// <summary>
        /// Add a text into the default category of the chosen language.
        /// </summary>
        /// <param name="lcid">Language that the text should be added to</param>
        /// <param name="category">Category that the text should be added to. Use null for default category.</param>
        /// <param name="textName">Name used to identify the text.</param>
        /// <param name="phrase">Text to add</param>
        /// <example>
        /// langMgr.Add(1053, "Validator", "Required", "{0} is required.");
        /// </example>
        void Add(string textName, int lcid, string category, string phrase);

        /// <summary>
        /// Get a phrase for the current language from the default category.
        /// </summary>
        /// <param name="textName">Phrase to find.</param>
        /// <returns>text if found; string.Empty if not.</returns>
        /// <remarks>Uses .CurrentThread.CurrentCulture.LCID to determine the current language. Will use DefaultLcid if current language is not found.</remarks>
        /// <example>
        /// string errorMsg = langMgr["Required"];
        /// </example>
        string this[string textName] { get; }

        /// <summary>
        /// Get a phrase for the current language.
        /// </summary>
        /// <param name="category">Categories are used to categorize all texts, the default category is null.</param>
        /// <param name="textName">Phrase to find.</param>
        /// <returns>text if found; string.Empty if not.</returns>
        /// <remarks>Uses .CurrentThread.CurrentCulture.LCID to determine the current language. Will use DefaultLcid if current language is not found.</remarks>
        /// <example>
        /// string errorMsg = langMgr["Validator", "Required"];
        /// </example>
        string this[string textName, string category] { get; }

        /// <summary>
        /// Get a phrase for the current language.
        /// </summary>
        /// <param name="category">Categories are used to categorize all texts, the default category is null.</param>
        /// <param name="textName">Phrase to find.</param>
        /// <returns>text if found; string.Empty if not.</returns>
        /// <remarks>Uses .CurrentThread.CurrentCulture.LCID to determine the current language. Will use DefaultLcid if current language is not found.</remarks>
        /// <example>
        /// string errorMsg = langMgr["Validator", "Required"];
        /// </example>
        /// <param name="lcid">Language that phrase should be fetched from.</param>
        string this[string textName, string category, int lcid] { get; }

        /// <summary>
        /// Get a specifc language category.
        /// </summary>
        /// <param name="name">category name</param>
        /// <returns>Category if found; otherwise LanguageCategory.Empty.</returns>
        LanguageCategory GetCategory(string name);

        /// <summary>
        /// Number of categories
        /// </summary>
        int Count
        { get; }
    }


    
}