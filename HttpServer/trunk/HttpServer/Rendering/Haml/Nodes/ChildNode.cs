using HttpServer.Rendering.Haml.Nodes;
using HttpServer.Rendering.Haml;

namespace HttpServer.Rendering.Haml.Nodes
{
    public abstract class ChildNode : Node
    {
        public ChildNode(Node parent) : base(parent)
        {
            
        }

        public Node AddMe(NodeList prototypes, Node parent, LineInfo line, Node me)
        {
            if (parent == null)
            {
                TagNode tag = (TagNode)prototypes.CreateNode("%", parent);
                tag.Name = "div";
                tag.LineInfo = line;
                tag.AddModifier(me);
                return tag;
            }

            return me;
        }

        protected int GetEndPos(int offset, string line, char terminator)
        {
            // find string to parse
            bool inQuote = false;
            for (int i = offset + 1; i < line.Length; ++i)
            {
                char ch = line[i];
                if (ch == '"')
                    inQuote = !inQuote;
                else if (ch == terminator && !inQuote)
                    return i;
            }

            return -1;
        }

        public override bool IsTextNode
        {
            get { return false; }
        }

        protected int GetEndPos(int offset, string line)
        {
            // find string to parse
            bool inQuote = false;
            for (int i = offset + 1; i < line.Length; ++i)
            {
                char ch = line[i];
                if (ch == '"')
                    inQuote = !inQuote;
                else if (!char.IsLetterOrDigit(ch) && !inQuote && ch != '_')
                    return i;
            }

            return -1;
        }
    }
}