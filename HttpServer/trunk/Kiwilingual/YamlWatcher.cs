using System;
using System.IO;
using System.Timers;
using Kiwilingual.Textfiles;

namespace Kiwilingual
{
	/// <summary>
	/// Class to subscribe to changeevents on a file and update a languagehierarchy every time the file changes.
	/// </summary>
	/// <remarks>If the file being watched is deleted or renamed no more changes will be made to the languagenode</remarks>
    public class YamlWatcher : IDisposable
    {
        public static char PathSeparator = '\\';
        private readonly string _fileName;
        private readonly string _path;
		private readonly string _fullName;
        private FileSystemWatcher _watcher;
        private readonly LanguageNode _languageNode;
		
		/// <summary>A timer to use as delay if the file is currently in use</summary>
		private readonly Timer _readTimer = new Timer(500);

		/// <summary>
		/// Instantiates the class to listen to changes in a file, also reads the file and fills the languageNode with language data
		/// </summary>
		/// <param name="languageNode">LanguageNode to fill</param>
		/// <param name="filename">The filename to watch</param>
        public YamlWatcher(LanguageNode languageNode, string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename + " do not exist.", filename);

			_path = Path.GetDirectoryName(Path.GetFullPath(filename));
			_fileName = Path.GetFileName(filename);
			_fullName = _path + "\\" + _fileName;
            _languageNode = languageNode;

			LoadFile(filename, _languageNode);
            Watch();

			_readTimer.Elapsed += OnTryRead;
        }

		/// <summary>
		/// Callback for when the file should be read again
		/// </summary>
		void OnTryRead(object sender, ElapsedEventArgs e)
		{
			_readTimer.Stop();
			LoadFile();
		}

		/// <summary>
		/// Sets the class to watch the specified file
		/// </summary>
        private void Watch()
        {
            _watcher = new FileSystemWatcher(_path, _fileName);
            _watcher.EnableRaisingEvents = true;
            _watcher.Changed += OnFileChanged;
        }

		/// <summary>
		/// Callback for when the file changes
		/// </summary>
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            LoadFile();
        }

		/// <summary>
		/// Private loading of file to wrap read failure control
		/// </summary>
		private void LoadFile()
		{
			try
			{
				LoadFile(_fullName, _languageNode);
			}
			catch (IOException)
			{
				_readTimer.Start();
			}
		}

		/// <summary>
		/// Fill the rootNode with languages and categories from the specified file
		/// </summary>
		/// <param name="fullPath">Full file path</param>
		/// <param name="rootNode">The rootNode to fill</param>
		/// <exception cref="ArgumentException">If rootNode is of type EmptyLanguageNode</exception>
		/// <exception cref="ArgumentNullException">If rootNode or fullPath is null</exception>
        public static void LoadFile(string fullPath, LanguageNode rootNode)
        {
			if (string.IsNullOrEmpty(fullPath))
				throw new ArgumentNullException("fullPath");

			if (rootNode == null)
				throw new ArgumentNullException("rootNode");

			if (rootNode is EmptyLanguageNode)
				throw new ArgumentException("rootNode must not be of type EmptyLanguageNode", "rootNode");

			// Clear out any old information in the language nodes
			rootNode.ClearHierarchy();

			//TextReader reader = new StreamReader(fullPath);
			using (Stream stream = Stream.Synchronized(new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.None)))
			{
				TextReader reader = new StreamReader(stream);
				YamlLight yaml = YamlLight.Parse(reader);

				// go through all languages
				foreach (YamlLight language in yaml)
				{
					int lcid;
					if (int.TryParse(language.Name, out lcid))
					{
						// go through all textstrings / categories for each language
						foreach (YamlLight childNode in language)
						{
							// if the entry contains more than one line of text it is a sub root node and not just an entry in the root
							if (childNode.Count > 0)
							{
								// try to retrieve the node from the root in case it's been instantiated before otherwise create it then expand
								LanguageNode languageNode = rootNode.GetNode(childNode.Name);
								if (languageNode is EmptyLanguageNode)
								{
									languageNode = rootNode.AddNode(childNode.Name);
								}
								foreach (YamlLight child in childNode)
									Expand(child, languageNode, lcid);

								if (!string.IsNullOrEmpty(childNode.Value))
									rootNode.Add(childNode.Name, lcid, childNode.Value);
							}
							else
								rootNode.Add(childNode.Name, lcid, childNode.Value);
						}

					}
				}

				reader.Dispose();
			}
        }

		private static void Expand(YamlLight node, LanguageNode parent, int lcid)
		{
			if(node.Count > 0)
			{
				LanguageNode langNode = parent.GetNode(node.Name);
				if (langNode is EmptyLanguageNode)
					langNode = parent.AddNode(node.Name);

				if (!string.IsNullOrEmpty(node.Value))
					parent.Add(node.Name, lcid, node.Value);

				foreach (YamlLight subNode in node)
					Expand(subNode, langNode, lcid);
			}
			else
				parent.Add(node.Name, lcid, node.Value);
		}

		#region IDisposable members
		/// <summary>
		/// Function to stop the watcher
		/// </summary>
		public void Dispose()
		{
			if (_watcher != null)
			{
				_watcher.Dispose();
				_watcher = null;
			}
		}
		#endregion
	}
}
