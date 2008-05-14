using System.Collections.Generic;
using HttpServer.Rendering;
using HttpServer.Rendering.Haml;

namespace HttpServer.Rendering.Haml.Nodes
{
    public abstract class Node
    {
        private LinkedList<Node> _children = new LinkedList<Node>();
        private LinkedList<Node> _modifiers = new LinkedList<Node>();
        private LineInfo _lineInfo;
        private readonly Node _parent;
        
        public Node(Node parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Text nodes should be added as child.
        /// </summary>
        public abstract bool IsTextNode
    {
        get;
    }

        /// <summary>
        /// Count our children and our childrens children and so on...
        /// </summary>
        public int AllChildrenCount
        {
            get
            {
                int count = _children.Count;

                foreach (Node node in _children)
                    count += node.AllChildrenCount;

                return count;
            }
        }

        public LinkedList<Node> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        /// <summary>
        /// Should only be set for each parent.
        /// </summary>
        public LineInfo LineInfo
        {
            get { return _lineInfo; }
            set { _lineInfo = value; }
        }

        protected LinkedList<Node> Modifiers
        {
            get { return _modifiers; }
        }

        public void AddModifier(Node node)
        {
            if (node.IsTextNode)
                _children.AddLast(node);
            else
                _modifiers.AddLast(node);
        }

        public Node LastModifier
        {
            get { return _modifiers.Last.Value; }
        }

        public int ModifierCount
        {
            get { return _modifiers.Count; }
        }

        /// <summary>
        /// Parent node.
        /// </summary>
        public Node Parent
        {
            get { return _parent; }
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
        public abstract Node Parse(NodeList prototypes, Node parent, LineInfo line, ref int offset);

        /// <summary>
        /// determines if this node can handle the line (by checking the first word);
        /// </summary>
        /// <param name="word">Controller char (word)</param>
        /// <returns>true if text belongs to this node type</returns>
        /// <param name="firstNode">First node on line, used since some nodes cannot exist on their own on a line.</param>
        public abstract bool CanHandle(string word, bool firstNode);

        public abstract string ToHtml();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inString">true if we are inside the internal stringbuilder</param>
        /// <returns></returns>
        public virtual string ToCode(ref bool inString)
        {
            return ToCode(ref inString, false, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smallEnough">code is small enough to fit on one row</param>
        /// <param name="inString">true if we are inside the internal stringbuilder</param>
        /// <returns></returns>
        public virtual string ToCode(ref bool inString, bool smallEnough)
        {
            return ToCode(ref inString, smallEnough, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inString">true if we are inside the internal stringbuilder</param>
        /// <param name="smallEnough">code is small enough to fit on one row.</param>
        /// <param name="defaultValue">smallEnough is a default value, recalc it</param>
        /// <returns></returns>
        protected abstract string ToCode(ref bool inString, bool smallEnough, bool defaultValue);

        /// <summary>
        /// Get intendation level for this node.
        /// </summary>
        /// <returns></returns>
        protected int GetIntendation()
        {
            if (LineInfo == null)
            {
                if (Parent == null)
                    return 0;
                else
                    return Parent.GetIntendation() + 1;
            }
            else
                return LineInfo.Intendation;
        }

    }
}
