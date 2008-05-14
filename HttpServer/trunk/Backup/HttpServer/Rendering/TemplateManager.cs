using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpServer.Rendering
{
    /// <summary>
    /// Purpose if this class is to take template objects and keep them in
    /// memory. It will also take a filename and the code generator to use
    /// if when the template have been changed on disk.
    /// </summary>
    public class TemplateManager
    {
        private readonly Dictionary<string, TemplateInfo> _compiledTemplates = new Dictionary<string, TemplateInfo>();
        private readonly Dictionary<string, TemplateGenerator> _generators = new Dictionary<string, TemplateGenerator>();
        private readonly List<Type> _includedTypes = new List<Type>();


        /// <summary>
        /// Add a template generator
        /// </summary>
        /// <param name="fileExtension">File extension without the dot.</param>
        /// <param name="generator">Generator to handle the extension</param>
        /// <exception cref="InvalidOperationException">If the generator already exists.</exception>
        /// <exception cref="ArgumentException">If file extension is incorrect</exception>
        /// <exception cref="ArgumentNullException">If generator is not specified.</exception>
        /// <example>
        /// cache.Add("haml", new HamlGenerator());
        /// </example>
        public void Add(string fileExtension, TemplateGenerator generator)
        {
            if (string.IsNullOrEmpty(fileExtension) || fileExtension.Contains("."))
                throw new ArgumentException("Invalid file extension.");
            if (generator == null)
                throw new ArgumentNullException("generator");

            if (_generators.ContainsKey(fileExtension))
                throw new InvalidOperationException("A generator already exists for " + fileExtension);

            _generators.Add(fileExtension, generator);
        }

        /// <summary>
        /// This type should be included, so it may be called from the scripts (namespace and assembly).
        /// </summary>
        /// <param name="type"></param>
        public void AddType(Type type)
        {
            bool assemblyExists = false;
            bool nsExists = false;
            foreach (Type includedType in _includedTypes)
            {
                if (includedType.Namespace == type.Namespace)
                    nsExists = true;
                if (includedType.Assembly == type.Assembly)
                    assemblyExists = true;
                if (nsExists && assemblyExists)
                    break;
            }

            if (!assemblyExists || !nsExists)
                _includedTypes.Add(type);
        }

        /// <summary>
        /// Checks the template.
        /// </summary>
        /// <param name="info">Template information, filename must be set.</param>
        /// <returns>true if template exists and have been compiled.</returns>
        private static bool CheckTemplate(TemplateInfo info)
        {
            if (info == null)
                return false;

            if (File.Exists(info.Filename))
            {
                if (File.GetLastWriteTime(info.Filename) < info.CompiledWhen)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Compiles the specified code.
        /// </summary>
        /// <param name="code">c# code generated from a template.</param>
        /// <param name="arguments">Arguments as in name, value, name, value, name, value</param>
        /// <returns>Template</returns>
        /// <exception cref="CompileException">If compilation fails</exception>
        /// <exception cref="ArgumentException">If a parameter is incorrect</exception>
        protected TinyTemplate Compile(string code, params object[] arguments)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Code is not specified.");
            if (arguments.Length%2 != 0)
                throw new ArgumentException("Arguments should be added as: name, value, name, value [.....]");

            TemplateCompiler compiler = new TemplateCompiler();
            foreach (Type type in _includedTypes)
                compiler.AddType(type);

            return compiler.Compile(arguments, code);
        }

        /// <summary>
        /// Will generate code from the template.
        /// Next step is to compile the code.
        /// </summary>
        /// <param name="fullPath">Path and filename to template.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidOperationException">If no template generator exists for the specified extension.</exception>
        /// <exception cref="CodeGeneratorException">If parsing/compiling fails</exception>
        /// <see cref="Render"/>
        protected string GenerateCode(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentException("No filename was specified.");

            int pos = fullPath.LastIndexOf('.');
            if (pos == -1)
                throw new ArgumentException("Filename do not contain a file extension.");
            if (pos == fullPath.Length - 1)
                throw new ArgumentException("Invalid filename '" + fullPath + "', should not end with a dot.");

            string extension = fullPath.Substring(pos + 1);


            lock (_generators)
            {
                try
                {
                    TemplateGenerator generator = null;
                    if (extension == "*")
                        generator = GetGeneratorForWildCard(ref fullPath);
                    else
                    {
                        if (!File.Exists(fullPath))
                            throw new FileNotFoundException("File '" + fullPath + "' do not exist.");
                        if (_generators.ContainsKey(extension))
                            generator = _generators[extension];
                    }

                    if (generator == null)
                        throw new InvalidOperationException("No template generator exists for '" + fullPath + "'.");

                    generator.Parse(fullPath);
                    StringBuilder sb = new StringBuilder();
                    using (TextWriter writer = new StringWriter(sb))
                    {
                        generator.GenerateCode(writer);
                        return sb.ToString();
                    }
                }
                catch (DirectoryNotFoundException err)
                {
                    throw new FileNotFoundException("Directory not found for: " + fullPath, err);
                }
                catch (PathTooLongException err)
                {
                    throw new FileNotFoundException("Path too long: " + fullPath, err);
                }
                catch (UnauthorizedAccessException err)
                {
                    throw new UnauthorizedAccessException("Failed to acces: " + fullPath, err);
                }
            }
        }

        /// <summary>
        /// Find a template using wildcards in filename.
        /// </summary>
        /// <param name="fullPath">Full path (including wildcards in filename) to where we should find a template.</param>
        /// <returns>First found generator if an extension was matched; otherwise null.</returns>
        /// <remarks>method is not thread safe</remarks>
        private TemplateGenerator GetGeneratorForWildCard(ref string fullPath)
        {
            int pos = fullPath.LastIndexOf('\\');
            if (pos == -1)
                throw new InvalidOperationException("Failed to find path in filename.");

            string path = fullPath.Substring(0, pos);
            string filename = fullPath.Substring(pos + 1);

            string[] files = Directory.GetFiles(path, filename);
            for (int i = 0; i < files.Length; ++i)
            {
                pos = files[i].LastIndexOf('.');
                string extension = files[i].Substring(pos + 1);

                if (_generators.ContainsKey(extension))
                {
                    fullPath = files[i];
                    return _generators[extension];
                }
            }

            return null;
        }

        /// <summary>
        /// Generate HTML from a template.
        /// </summary>
        /// <param name="filename">Path and filename</param>
        /// <param name="args">Variables used in the template. Should be specified as "name, value, name, value" where name is variable name and value is variable contents.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="CompileException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <example>
        /// string html = cache.Generate("views\\users\\view.haml", "user", dbUser, "isAdmin", dbUser.IsAdmin);
        /// </example>
        public string Render(string filename, params object[] args)
        {
            TemplateInfo info;
            lock (_compiledTemplates)
            {
                if (_compiledTemplates.ContainsKey(filename))
                    info = _compiledTemplates[filename];
                else
                {
                    info = new TemplateInfo();
                    info.Filename = filename;
                    info.Template = null;
                    info.CompiledWhen = DateTime.MinValue;
                    _compiledTemplates.Add(filename, info);
                }
            }

            lock (info)
            {
                if (!CheckTemplate(info))
                {
                    string code = GenerateCode(filename);
                    info.Template = Compile(code, args);
                    info.CompiledWhen = DateTime.Now;
                }

                return info.Template.Invoke(args);
            }
        }

        #region Nested type: TemplateInfo

        /// <summary>
        /// Keeps information about templates, so we know when to regenerate it.
        /// </summary>
        private class TemplateInfo
        {
            private DateTime _compiledWhen;
            private string _filename;
            private TinyTemplate _template;

            public DateTime CompiledWhen
            {
                get { return _compiledWhen; }
                set { _compiledWhen = value; }
            }

            public string Filename
            {
                get { return _filename; }
                set { _filename = value; }
            }

            public TinyTemplate Template
            {
                get { return _template; }
                set { _template = value; }
            }
        }

        #endregion
    }
}