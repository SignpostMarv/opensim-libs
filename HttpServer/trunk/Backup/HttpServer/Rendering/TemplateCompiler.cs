using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace HttpServer.Rendering
{
    /// <summary>
    /// The compiler is responsible of creating a render object which can be
    /// cached and used over and over again.
    /// </summary>
    /// <seealso cref="TemplateManager"/>
    /// <seealso cref="TemplateGenerator"/>
    public class TemplateCompiler
    {
        private readonly List<string> _assemblies = new List<string>();
        private readonly List<string> _namespaces = new List<string>();
        private string _generatedTemplate;

        public static string TemplateBase =
            @"{namespaces}

namespace Tiny.Templates {
    class TemplateClass :  TinyTemplate
    {
        {members}

        public string Invoke(object[] args)
        {   
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            {body}

            return sb.ToString();
        }
    }
}";

        public TemplateCompiler()
        {
            AddAssembly(GetType());
            AddAssembly(typeof(TinyTemplate));
            AddNamespace(typeof(TinyTemplate));
        }


        /// <summary>
        /// Compiles the specified args.
        /// </summary>
        /// <param name="args">Arguments, should contain "name, value, name, value" etc.</param>
        /// <param name="template">c# code that will be included in the generated template class</param>
        /// <returns>Tiny template if successful; otherwise null.</returns>
        /// <exception cref="CompileException">If compilation fails</exception>
        /// <exception cref="ArgumentException">If args are incorrect</exception>
        public TinyTemplate Compile(object[] args, string template)
        {
            if (args.Length % 2 != 0)
                throw new ArgumentException("Args must consist of string, object, string, object, string, object as key/value couple.");

            for (int i = 0; i < args.Length; i += 2)
            {
                if (args[i + 1] == null)
                    throw new CompileException("No arguments may be null during template compilation, they are used to identify which assemblies/namespaces to include. Please correct parameter '" + args[i] + "'.");

                AddAssembly(args[i + 1].GetType());
                AddNamespace(args[i + 1].GetType());
            }

            string namespaces = string.Empty;
            foreach (string s in _namespaces)
                namespaces += "using " + s + ";" + Environment.NewLine;

            string members = string.Empty;
            for (int i = 0; i < args.Length; i += 2)
                members += GetTypeName(args[i + 1].GetType()) + " " + args[i] + ";" + Environment.NewLine;

            string body = string.Empty;
            for (int i = 1; i < args.Length; i += 2)
                body += "this." + args[i - 1] + " = (" + GetTypeName(args[i].GetType()) + ")args[" + i + "];" + Environment.NewLine;

            body += template;

            _generatedTemplate = TemplateBase.Replace("{namespaces}", namespaces).Replace("{members}", members).Replace("{body}", body);
            return Compile();
        }


        /// <summary>
        /// This type should be included, so it may be called from the scripts (namespace and assembly).
        /// </summary>
        /// <param name="type"></param>
        public void AddType(Type type)
        {
            AddNamespace(type);
            AddAssembly(type);
        }

        private void AddNamespace(Type type)
        {
            string ns = type.Namespace;
            bool found = false;
            foreach (string s in _namespaces)
            {
                if (string.Compare(s, ns, true) == 0)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                _namespaces.Add(ns);

            foreach (Type argument in type.GetGenericArguments())
                AddNamespace(argument);
        }

        private void AddAssembly(Type type)
        {
            string path = type.Assembly.Location;
            bool found = false;
            foreach (string s in _assemblies)
            {
                if (string.Compare(s, path, true) == 0)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                _assemblies.Add(path);

            foreach (Type argument in type.GetGenericArguments())
                AddNamespace(argument);
        }

        /// <summary>
        /// Used to get correct names for generics.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetTypeName(Type type)
        {
            string typeName = type.Name;
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(typeName.Substring(0, typeName.IndexOf('`')));
                sb.Append("<");
                bool first = true;
                foreach (Type genericArgumentType in type.GetGenericArguments())
                {
                    if (!first)
                        sb.Append(", ");
                    first = false;
                    sb.Append(GetTypeName(genericArgumentType));
                }
                sb.Append(">");
                return sb.ToString();
            }
            else
                return typeName;
        }


        /// <summary>
        /// Compiles a C# class.
        /// </summary>
        /// <returns>TinyTemplate if successful; otherwise null.</returns>
        /// <exception cref="CompileException">If compilation failes</exception>
        private TinyTemplate Compile()
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            foreach (string assembly in _assemblies)
                parameters.ReferencedAssemblies.Add(ResolveAssemblyPath(assembly));

            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, _generatedTemplate);
            if (results.Errors.Count > 0)
            {
                string errs = "";

                foreach (CompilerError CompErr in results.Errors)
                {
                    errs += "Template: " + CompErr.FileName + Environment.NewLine +
                        "Line number: " + CompErr.Line + Environment.NewLine +
                        "Error: " + CompErr.ErrorNumber + " '" + CompErr.ErrorText + "'";
                }
                CompileException err = new CompileException(errs);
                err.Data.Add("code", _generatedTemplate);
                throw err;
            }

            Assembly generatorAssembly = results.CompiledAssembly;
            return (TinyTemplate)generatorAssembly.CreateInstance("Tiny.Templates.TemplateClass", false, BindingFlags.CreateInstance, null, null, null, null);
        }

        private string ResolveAssemblyPath(string name)
        {
            if (name.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                return name;

            name = name.ToLower();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (IsDynamicAssembly(assembly))
                {
                    continue;
                }

                if (Path.GetFileNameWithoutExtension(assembly.Location).ToLower().Equals(name))
                {
                    return assembly.Location;
                }
            }

            string foo = name.Substring(name.Length - 4, 1);
            if (!(foo.Equals(".")))
                name += ".dll";
            //return Path.GetFullPath(name);
            return Path.GetFullPath(name);
        }

        private bool IsDynamicAssembly(Assembly assembly)
        {
            return assembly.ManifestModule.Name.StartsWith("<");
        }

    }

    public class CompileException : Exception
    {
        public CompileException(string errorMessage) : base(errorMessage)
        {
        }
    }
}
