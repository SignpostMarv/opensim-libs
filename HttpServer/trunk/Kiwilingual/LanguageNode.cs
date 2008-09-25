using System;
using System.Collections.Generic;

namespace Kiwilingual
{
	/// <summary>
	/// The LanguageNode provides a base class for different implementations of a hierachial language structure
	/// </summary>
    public abstract class LanguageNode
    {
        public static EmptyLanguageNode EmptyLanguageNode = new EmptyLanguageNode(0);

        private int _defaultLCID;

		private string _name = string.Empty;

    	private readonly Dictionary<string, LanguageNode> _subNodes = new Dictionary<string, LanguageNode>();
		protected LanguageNode _parentNode = null;

		public LanguageNode ParentNode
    	{
			get { return _parentNode; }
			internal set { _parentNode = value; }
    	}

		public Dictionary<string, LanguageNode> SubNodes
		{
			get { return _subNodes; }
		}

        public LanguageNode(int defaultLCID)
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
		/// Adds a sub category
		/// </summary>
		/// <param name="name">Name of the sub category</param>
		/// <exception cref="ArgumentException">If a category with the specified name already exists</exception>
		/// <exception cref="ArgumentNullException">If name is null</exception>
    	public abstract LanguageNode AddNode(string name);

		/// <summary>
		/// Retrieves a subcategory
		/// </summary>
		/// <param name="name">The category name</param>
		/// <returns>Null if the category does not exist</returns>
		/// <exception cref="ArgumentNullException">If name is null</exception>
    	public LanguageNode GetNode(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			if(!_subNodes.ContainsKey(name))
				return EmptyLanguageNode;

			return _subNodes[name];
		}

		/// <summary>
		/// Retrieves a subnode or null if the requested subnode does not exist
		/// </summary>
		/// <param name="name">Name of the parameter</param>
		/// <returns>The named LanguageNode or null</returns>
		/// <exception cref="ArgumentNullException">If name is null or empty</exception>
		public LanguageNode GetNodeUnsafe(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			if(_subNodes.ContainsKey(name))
				return _subNodes[name];

			return null;
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
		/// Returns the name of the node
		/// </summary>
		public string Name
		{
			get { return _name; }
			internal set{ _name = value;}
		}

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

		/// <summary>Empties all saved values in the node and its subnodes</summary>
		internal abstract void ClearHierarchy();
    }


    public class EmptyLanguageNode : LanguageNode
    {
        public EmptyLanguageNode(int defaultLCID)
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

		/// <summary>Unimplemented function to fullfill the requirements of LanguageNode base class</summary>
		/// <param name="name">The name to add</param>
		public override LanguageNode AddNode(string name)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>Empties all saved values in the node and its subnodes</summary>
		internal override void ClearHierarchy()
		{
			throw new System.NotImplementedException();
		}
	}
}
