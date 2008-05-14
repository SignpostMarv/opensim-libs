using System.IO;
using HttpServer.Language.Textfiles;

namespace HttpServer.Language
{
    public class YamlWatcher
    {
        public static char PathSeparator = '\\';
        private readonly string _fileName;
        private readonly string _path;
        private FileSystemWatcher _watcher;
        private LanguageManager _langMgr;

        public YamlWatcher(LanguageManager langMgr, string fullPath)
        {
            if (!File.Exists(fullPath))
                throw new FileNotFoundException(fullPath + " do not exist.", fullPath);

            int pos = fullPath.LastIndexOf(PathSeparator);
            if (pos == -1)
                throw new FileNotFoundException("FullPath do not contain a file name (failed to find Path Separator).", fullPath);

            _path = fullPath.Substring(0, pos);
            _fileName = fullPath.Substring(pos + 1);
            _langMgr = langMgr;

            Watch();
        }

        public YamlWatcher()
        {
            
        }

        private void Watch()
        {
            _watcher = new FileSystemWatcher(_path, _fileName);
            _watcher.EnableRaisingEvents = true;
            _watcher.Changed += OnFileChanged;
            _watcher.BeginInit();
            _watcher.EndInit();
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            LoadFile(e.FullPath, _langMgr);   
        }

        public static void LoadFile(string fullPath, LanguageManager langMgr)
        {
            TextReader reader = new StreamReader(fullPath);
            YamlLight yaml = YamlLight.Parse(reader);

            // go through all languages
            foreach (YamlLight node in yaml)
            {
                int lcid;
                if (int.TryParse(node.Name, out lcid))
                {
                    // go through all textstrings / categories
                    foreach (YamlLight category in node)
                    {
                        // categories have children, texts in general category do not.
                        if (category.Count > 0)
                            foreach (YamlLight text in category)
                                langMgr.Add(text.Name, lcid, category.Name, text.Value);
                        else
                            langMgr.Add(category.Name, lcid, category.Value);
                    }
                        
                }
            }
        }
    }
}
