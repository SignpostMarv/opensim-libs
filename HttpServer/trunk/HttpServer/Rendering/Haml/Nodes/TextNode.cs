using System;
using System.Text;
using HttpServer.Rendering;
using HttpServer.Rendering.Haml;

namespace HttpServer.Rendering.Haml.Nodes
{
    /// <summary>
    /// A text only node.
    /// </summary>
    public class TextNode : Node
    {
        private string _text;

        public TextNode(Node parent, string text) : base(parent)
        {
            Text = text;
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public override bool IsTextNode
        {
            get { return true; }
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
            // text on tag rows are identified by a single space.
            if (parent != null && line.Data[offset] == ' ')
                ++offset;

            TextNode node = new TextNode(parent, line.Data.Substring(offset));
            if (parent == null)
                node.LineInfo = line;
            offset = line.Data.Length;
            return node;
        }

        /// <summary>
        /// determines if this node can handle the line (by checking the first word);
        /// </summary>
        /// <param name="word">Controller char (word)</param>
        /// <returns>true if text belongs to this node type</returns>
        public override bool CanHandle(string word, bool firstNode)
        {
            return word.Length > 0 && (char.IsWhiteSpace(word[0]));
        }

        public override string ToHtml()
        {
            // lineinfo = first node on line
            if (LineInfo != null)
                return string.Empty.PadLeft(GetIntendation(), '\t') + _text + Environment.NewLine;
            else
                return _text;
        }

        protected override string ToCode(ref bool inString, bool smallEnough, bool defaultValue)
        {
            int intendCount = GetIntendation();
            string intend = string.Empty.PadLeft(intendCount, '\t');
            string text = _text.Replace("\"", "\"\"");

            StringBuilder sb = new StringBuilder();
            if (!inString)
            {
                sb.Append("sb.Append(@\"");
                inString = true;
            }

            if (Children.Count > 0)
            {
                sb.Append(intend);
                sb.AppendLine(text);

                foreach (Node node in Children)
                    sb.Append(node.ToCode(ref inString, smallEnough));

            }
            else
            {
                // lineinfo = first node on line
                if (LineInfo != null && !smallEnough)
                    sb.Append(intend + text + Environment.NewLine);
                else
                    sb.Append(text);
                
            }

            return sb.ToString();
        }

    }
}
