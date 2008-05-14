using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HttpServer.Rendering;
using HttpServer.Rendering.Haml.Nodes;
using HttpServer.Rendering.Haml.Rules;

namespace HttpServer.Rendering.Haml
{
    /// <summary>
    /// Generates C#/HTML from haml code.
    /// 
    /// Haml doc: http://haml.hamptoncatlin.com/docs/rdoc/classes/Haml.html
    /// </summary>
    public class HamlGenerator : TemplateGenerator
    {
        private LineInfo _mother;
        private LineInfo _currentLine;
        private int _lineNo = -1;
        private LineInfo _prevLine;
        private TextReader _reader;
        public List<Rule> _rules = new List<Rule>();
        private Node _parentNode;

        public HamlGenerator()
        {
            _rules.Add(new NewLineRule());
            _rules.Add(new AttributesRule());
        }

        protected void CheckIntendation(LineInfo line, out int ws, out int intendation)
        {
            intendation = 0;
            ws = -1;

            char prevUnusedCh = line.UnparsedData[0];
            if (prevUnusedCh == '\t')
            {
                ++intendation;
                prevUnusedCh = char.MinValue;
            }
            else if (prevUnusedCh != ' ')
            {
                ws = 0;
                return;
            }

            for (int i = 1; i < line.UnparsedData.Length; ++i)
            {
                char ch = line.UnparsedData[i];

                if (ch == ' ')
                {
                    if (prevUnusedCh == '\t')
                    {
                        ++intendation;
                        prevUnusedCh = ' ';
                        continue;
                    }
                    else if (prevUnusedCh == ' ')
                    {
                        prevUnusedCh = char.MinValue;
                        ++intendation;
                        continue;
                    }
                    else
                        prevUnusedCh = ' ';
                }
                else if (ch == '\t')
                {
                    if (prevUnusedCh == ' ')
                        throw new CodeGeneratorException(line.LineNumber,
                                                 "Invalid intendation sequence: One space + one tab. Should either be one tab or two spaces.");
                    else if (prevUnusedCh == char.MinValue)
                    {
                        ++intendation;
                        prevUnusedCh = char.MinValue;
                        continue;
                    }
                }
                else
                {
                    if (prevUnusedCh != char.MinValue)
                        throw new CodeGeneratorException(line.LineNumber,
                                                 "Invalid intendation at char " + i + ", expected a space.");
                    else
                    {
                        if (i == 1 && !char.IsWhiteSpace(line.UnparsedData[0]))
                            ws = 0;
                        else
                            ws = i;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Parse a file and convert into to our own template object code.
        /// </summary>
        /// <param name="fullPath">Path and filename to a template</param>
        /// <exception cref="CodeGeneratorException">If something is incorrect in the template.</exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Parse(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentException("Path must be specified.", "fullPath");

                Stream stream = File.OpenRead(fullPath);
                TextReader reader = new StreamReader(stream);
                Parse(reader);
                reader.Close();
                reader.Dispose();
                stream.Dispose();
        }

        /// <summary>
        /// Parse a file and convert into to our own template object code.
        /// </summary>
        /// <param name="reader">A textreader containing our template</param>
        /// <exception cref="CodeGeneratorException">If something is incorrect in the template.</exception>
        public void Parse(TextReader reader)
        {
            _reader = reader;
            _mother = new LineInfo(-1, string.Empty);

            PreParse(reader);

            NodeList prototypes = new NodeList();
            prototypes.Add(new AttributeNode(null));
            prototypes.Add(new TagNode(null));
            prototypes.Add(new IdNode(null));
            prototypes.Add(new SilentCodeNode(null));
            prototypes.Add(new ClassNode(null));
            prototypes.Add(new DisplayCodeNode(null));
            prototypes.Add(new DocTypeTag(null, null));
            TextNode textNode = new TextNode(null, "prototype");
            _parentNode = new TextNode(null, string.Empty);

            foreach (LineInfo info in _mother.Children)
                ParseNode(info, prototypes, _parentNode, textNode);

        }

        /// <summary>
        /// Generate C# code from the template.
        /// </summary>
        /// <param name="writer">A textwriter that the generated code will be written to.</param>
        /// <exception cref="InvalidOperationException">If the template have not been parsed first.</exception>
        /// <exception cref="CodeGeneratorException">If template is incorrect</exception>
        public void GenerateCode(TextWriter writer)
        {
            writer.Write("sb.Append(@\"");
            bool inString = true;
            foreach (Node child in _parentNode.Children)
                writer.Write(child.ToCode(ref inString));
            writer.Write("\");");
        }

        /// <summary>
        /// Generate html code from the template.
        /// Code is encapsed in &lt;% and &lt;%=
        /// </summary>
        /// <param name="writer">A textwriter that the generated code will be written to.</param>
        /// <exception cref="InvalidOperationException">If the template have not been parsed first.</exception>
        /// <exception cref="CodeGeneratorException">If template is incorrect</exception>
        public void GenerateHtml(TextWriter writer)
        {
            foreach (Node child in _parentNode.Children)
                writer.Write(child.ToHtml());
        }

        protected void ParseNode(LineInfo theLine, NodeList prototypes, Node parent, TextNode textNode)
        {
            Node curNode = null;
            int offset = 0;

            // parse each part of a line
            while (offset <= theLine.Data.Length - 1)
            {
                Node node = prototypes.GetPrototype(GetWord(theLine.Data, offset), curNode == null);
                if (node == null)
                    node = textNode;

                node = node.Parse(prototypes, curNode, theLine, ref offset);

                // first node on line, set it as current
                if (curNode == null)
                {
                    curNode = node;
                    curNode.LineInfo = theLine;
                    parent.Children.AddLast(node);
                }
                else
                    curNode.AddModifier(node); // append attributes etc.
            }

            foreach (LineInfo child in theLine.Children)
                ParseNode(child, prototypes, curNode, textNode);
        }

        /// <summary>
        /// Get the first word (letters and digits only) from the specified offset.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string GetWord(string data, int offset)
        {
            for (int i = offset; i < data.Length; ++i)
            {
                if (!char.IsLetterOrDigit(data[i]) && data[i] != '!')
                    return data.Substring(offset, i - offset + 1);
            }

            return data;
        }

        /// <summary>
        /// PreParse goes through the text add handles intendation
        /// and all multiline cases.
        /// </summary>
        /// <param name="reader">Reader contaning the text</param>
        protected void PreParse(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            // Read first line to be able to assign it to the mother.
            if (!ReadLine())
                throw new CodeGeneratorException(1, "No data.");

            if (_currentLine.Intendation != 0)
                throw new CodeGeneratorException(1, "Invalid intendation, should be 0.");
            _currentLine.Parent = _mother;
            CheckIntendation(_currentLine);
            CheckMultiLine(_prevLine, _currentLine);

            while (ReadLine())
            {
                if (_currentLine.UnparsedData.Length == 0)
                    continue;

                CheckIntendation(_currentLine);
                CheckMultiLine(_prevLine, _currentLine);
                if (_prevLine.AppendNextLine)
                {
                    _prevLine.Append(_currentLine);
                    _currentLine = _prevLine;
                    continue;
                }

                HandlePlacement();
            }            
        }

        private bool OnlyWhitespaces(string data)
        {
            foreach (char ch in data)
            {
                if (!char.IsWhiteSpace(ch))
                    return false;
            }

            return true;
        }

        protected void HandlePlacement()
        {
            // Check intendation so that we know where to place the line

            // Larger intendation = child
            if (_currentLine.Intendation > _prevLine.Intendation)
            {
                if (_currentLine.Intendation != _prevLine.Intendation + 1)
                    throw new CodeGeneratorException(_currentLine.LineNumber,
                                             "Too large intendation, " + (_currentLine.Intendation -
                                             _prevLine.Intendation) + " steps instead of 1.");

                _currentLine.Parent = _prevLine;
            }
            // same intendation = same parent.
            else if (_currentLine.Intendation == _prevLine.Intendation)
                _currentLine.Parent = _prevLine.Parent;

                // Node should be placed on a node up the chain.
            else
            {
                // go back until we find someone at the same level
                LineInfo sameLevelNode = _prevLine;
                while (sameLevelNode != null && sameLevelNode.Intendation > _currentLine.Intendation)
                    sameLevelNode = sameLevelNode.Parent;

                if (sameLevelNode == null)
                {
                    if (_currentLine.Intendation > 0)
                        throw new CodeGeneratorException(_currentLine.LineNumber, "Failed to find parent.");
                    else
                        _currentLine.Parent = _mother;
                }
                else
                    _currentLine.Parent = sameLevelNode.Parent;
            }            
        }

        protected void CheckIntendation(LineInfo line)
        {
            int ws, intendation;
            CheckIntendation(line, out ws, out intendation);
            if (ws == -1)
                throw new CodeGeneratorException(line.LineNumber, "Failed to find intendation");

            line.Set(ws, intendation);
        }

        protected void CheckMultiLine(LineInfo prevLine, LineInfo line)
        {
            
            foreach (Rule rule in _rules)
            {
                if (rule.IsMultiLine(line))
                {
                    Console.WriteLine(line.LineNumber + ": " + rule.GetType().Name + " says that the next line should be appended.");
                    line.AppendNextLine = true;
                    continue;
                }
            }
        }

        protected bool ReadLine()
        {
            string line = _reader.ReadLine();
            if (line == null)
                return false;

            ++_lineNo;
            _prevLine = _currentLine;
            _currentLine = new LineInfo(_lineNo, line);
            return true;
        }

        public void PrintDocument()
        {
            PrintNode(_mother);
        }

        public void PrintNode(LineInfo line)
        {
            Console.WriteLine(Spaces(line.Intendation) + line.Data);
            Debug.WriteLine(Spaces(line.Intendation) + line.Data);
            foreach (LineInfo info in line.Children)
                PrintNode(info);
        }

        public string Spaces(int count)
        {
            return "".PadLeft(count);
        }
    }
}