using System;
using System.Collections.Generic;
using HttpServer.Language.Yaml.Tags;

namespace HttpServer.Language.Yaml.Tags
{
    class Node : Tag
    {
        private string _key = string.Empty;
        private string _value = string.Empty;
        private Node _parent = null;
        private int _intend;
        private readonly LinkedList<Node> _children = new LinkedList<Node>();

        public Node(int intend)
        {
            if (intend < 0 || intend > 20)
                throw new ArgumentException("Invalid intendation", "intend");

            _intend = intend;
        }

        public Node(int intend, Node parent) : this(intend)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            _parent = parent;
        }

        public LinkedList<Node> Children
        {
            get { return _children; }
        }

        public Node Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public int Intend
        {
            get { return _intend; }
            set { _intend = value; }
        }
    }
}