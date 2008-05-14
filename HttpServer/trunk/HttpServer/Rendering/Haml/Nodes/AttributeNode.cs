using System.Collections.Specialized;
using System.Text;
using HttpServer.Rendering;
using HttpServer.Rendering.Haml.Nodes;
using HttpServer.Rendering.Haml;

namespace HttpServer.Rendering.Haml.Nodes
{
    public class AttributeNode : ChildNode
    {
        private NameValueCollection _attributes;

        public AttributeNode(Node parent, NameValueCollection col) : base(parent)
        {
            _attributes = col;
        }

        public AttributeNode(Node parent) : base(parent)
        {
            _attributes = new NameValueCollection();
        }

        public NameValueCollection Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
        /// Parse node contents add return a fresh node.
        /// </summary>
        /// <param name="prototypes">List containing all node types</param>
        /// <param name="parent">Node that this is a subnode to. Can be null</param>
        /// <param name="line">Line to parse</param>
        /// <param name="offset">Where to start the parsing. Should be set to where the next node should start parsing.</param>
        /// <returns>A node corresponding to the bla bla; null if parsing failed.</returns>
        /// <exception cref="CodeGeneratorException"></exception>
        public override Node Parse(NodeList prototypes, Node parent, LineInfo line, ref int offset)
        {
            if (line.Data[offset] != '{')
                throw new CodeGeneratorException(line.LineNumber, "Attribute cant handle info at char " + offset + 1);

            int endPos = GetEndPos(offset, line.Data, '}');
            if (endPos == -1)
                throw new CodeGeneratorException(line.LineNumber, "Failed to find end of attribute list.");

            NameValueCollection col = new NameValueCollection();
            string attributes = line.Data.Substring(offset + 1, endPos - offset - 1);
            ParseAttributes(line, attributes, col);
            offset = endPos + 1;


            AttributeNode node = (AttributeNode)prototypes.CreateNode("{", parent);
            node._attributes = col;
            return AddMe(prototypes, parent, line, node);
        }

        private static void ParseAttributes(LineInfo line, string attributes, NameValueCollection col)
        {
            bool inQuote = false;
            int parenthisCount = 0;
            string name = null;
            int start = -1;
            int step = 0; //0 = start of name, 1 = end of name, 2 = equal sign, 3 = start of value, 4 = end of value, 5 = comma
            for (int i = 0; i < attributes.Length; ++i)
            {
                char ch = attributes[i];

                if (ch == '"')
                {
                    inQuote = !inQuote;
                    if (inQuote && step == 3)
                    {
                        ++step;
                        start = i;
                    }
                }

                if (inQuote)
                    continue;

                if (ch == '(')
                    ++parenthisCount;
                if (ch == ')')
                    --parenthisCount;
                if (parenthisCount > 0)
                    continue;

                // find start of name
                if (step == 0)
                {
                    if (!char.IsWhiteSpace(ch))
                    {
                        start = i;
                        ++step;
                    }
                }
                    // find end of name
                else if (step == 1)
                {
                    if (char.IsWhiteSpace(ch) || ch == '=')
                    {
                        name = attributes.Substring(start, i - start);
                        start = -1;
                        ++step;
                    }
                }

                // find equal
                if (step == 2)
                {
                    if (ch == '=')
                        ++step;
                    continue;
                }

                // start of value
                if (step == 3)
                {
                    if (!char.IsWhiteSpace(ch))
                    {
                        start = i;
                        ++step;
                    }
                }

                    // end of value
                else if (step == 4)
                {
                    if (char.IsWhiteSpace(ch) || ch == ',')
                    {
                        AddAttribute(col, name, attributes.Substring(start, i - start));
                        start = -1;
                        ++step;
                    }
                }

                // find comma
                if (step == 5)
                {
                    if (ch == ',')
                        step = 0;

                    continue;
                }
            }

            if (step > 0 && step < 4)
                throw new CodeGeneratorException(line.LineNumber, "Invalid attributes");

            if (step == 4)
                AddAttribute(col, name, attributes.Substring(start, attributes.Length - start));
        }

        private static void AddAttribute(NameValueCollection col, string name, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            // need to end with a ", else parsing will fail.
            if (value[0] == '\"' && value[value.Length -1] != '\"')
                col.Add(name, value + "+ @\"\"");
            else 
                col.Add(name,value);
        }

        /// <summary>
        /// determines if this node can handle the line (by checking the first word);
        /// </summary>
        /// <param name="word">Controller char (word)</param>
        /// <returns>true if text belongs to this node type</returns>
        /// <param name="firstNode">first node on line</param>
        public override bool CanHandle(string word, bool firstNode)
        {
            if (word.Length >= 1 && word[0] == '{' && !firstNode)
                return true;

            return false;
        }

        public override string ToHtml()
        {
            StringBuilder attrs = new StringBuilder();
            for (int i = 0; i < Attributes.Count; ++i)
            {
                if (Attributes[i][0] != '"')
                    attrs.AppendFormat("{0}=<%= {1} %> ", Attributes.AllKeys[i], Attributes[i]);
                else
                    attrs.AppendFormat("{0}={1} ", Attributes.AllKeys[i], Attributes[i]);
            }

            return attrs.ToString();
        }

        protected override string ToCode(ref bool inString, bool smallEnough, bool defaultValue)
        {
            StringBuilder attrs = new StringBuilder();
            for (int i = 0; i < Attributes.Count; ++i)
            {
                if (Attributes[i][0] != '"')
                    attrs.AppendFormat("{0}=\"\"\"); sb.Append({1}); sb.Append(@\"\"\" ", Attributes.AllKeys[i], Attributes[i]);
                else
                    attrs.AppendFormat("{0}=\"{1}\" ", Attributes.AllKeys[i], Attributes[i]);
            }

            attrs.Length = attrs.Length - 1;
            return attrs.ToString();            
        }

    }
}