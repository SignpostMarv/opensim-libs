using HttpServer.Rendering;
using HttpServer.Rendering.Haml.Nodes;
using HttpServer.Rendering.Haml;

namespace HttpServer.Rendering.Haml.Nodes
{
    /// <summary>
    /// Represents a html class node.
    /// </summary>
    class ClassNode : ChildNode
    {
        private string _name;

        public ClassNode(Node parent) : base(parent)
        {            
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
            if (offset > line.Data.Length - 1)
                throw new CodeGeneratorException(line.LineNumber, "Too little data");

            int pos = GetEndPos(offset, line.Data);
            if (pos == -1)
                pos = line.Data.Length;

            ++offset;
            string name = line.Data.Substring(offset, pos - offset);
            offset = pos;

            ClassNode node = (ClassNode) prototypes.CreateNode(".", parent);
            node._name = name;
            return AddMe(prototypes, parent, line, node);
        }

        /// <summary>
        /// determines if this node can handle the line (by checking the first word);
        /// </summary>
        /// <param name="word">Controller char (word)</param>
        /// <returns>true if text belongs to this node type</returns>
        /// <param name="firstNode">first node on line</param>
        public override bool CanHandle(string word, bool firstNode)
        {
            return word.Length > 0 && word[0] == '.';
        }

        public override string ToHtml()
        {
            return "class=\"" + _name + "\" ";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inString">true if we are inside the internal stringbuilder</param>
        /// <param name="smallEnough">code is small enough to fit on one row.</param>
        /// <param name="defaultValue">smallEnough is a default value, recalc it</param>
        /// <returns></returns>
        protected override string ToCode(ref bool inString, bool smallEnough, bool defaultValue)
        {
            return "class=\"\"" + _name + "\"\"";
        }
    }
}