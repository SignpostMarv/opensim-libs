using System;

namespace HttpServer.Rendering
{
    /// <summary>
    /// Contains information on where in the template the error occurred, and what the error was.
    /// </summary>
    public class CodeGeneratorException : Exception
    {
        private readonly int _lineNumber;

        public CodeGeneratorException(int lineNumber, string error) : base(error)
        {
            _lineNumber = lineNumber;
        }

        /// <summary>
        /// Line number in template
        /// </summary>
        public int LineNumber
        {
            get { return _lineNumber; }
        }
    }
}