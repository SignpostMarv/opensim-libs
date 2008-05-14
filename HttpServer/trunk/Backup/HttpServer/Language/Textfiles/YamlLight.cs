using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace HttpServer.Language.Textfiles
{
    public class YamlLight : IEnumerable<YamlLight>
    {
        private string _name = string.Empty;
        private string _value = string.Empty;
        private readonly LinkedList<YamlLight> _children = new LinkedList<YamlLight>();
        private readonly YamlLight _parent = null;
        private readonly int _intendation = 0;

        public YamlLight(YamlLight parent, int intendation)
        {
            _parent = parent;
            _intendation = intendation;
            if (_parent != null)
                _parent._children.AddLast(this);
        }

        public YamlLight(YamlLight parent, string key, string value)
        {
            _parent = parent;
            _intendation = parent.Intendation+2;
            _name = key;
            _value = value;
            if (_parent != null)
                _parent._children.AddLast(this);
        }

        public int Count
        {
            get { return _children.Count; }
        }
        public int Intendation
        {
            get { return _intendation; }
        }

        public YamlLight Parent
        {
            get { return _parent; }
        }

        public void Add(string key, string value)
        {
            new YamlLight(this, key, value);
        }

        public YamlLight this[string name]
        {
            get
            {
                foreach (YamlLight node in _children)
                {
                    if (node.Name.Equals(name))
                        return node;
                }

                return null;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public static YamlLight Parse(TextReader reader)
        {
            YamlLight mother = new YamlLight(null, -1);

            int lineNumber = 0;
            string line = reader.ReadLine();
            YamlLight curNode = mother;
            while (line != null)
            {
                ++lineNumber;
                YamlLight node = HandlePlacement(curNode, line, lineNumber);
                if (node != null)
                {
                    curNode = node;
                    curNode.ParseData(line, curNode.Intendation, lineNumber);
                }

                line = reader.ReadLine();
            }

            return mother;
        }

        private static YamlLight HandlePlacement(YamlLight curNode, string line, int lineNumber)
        {
            if (line.Trim() == string.Empty)
                return null;
            int intend = GetIntend(line);
            if (intend == -1)
                throw new InvalidDataException("Invalid intendation on line " + lineNumber);

            YamlLight node;

            // new node is a child to current node
            if (curNode.Intendation < intend)
                node = new YamlLight(curNode, intend);
            // both are chilren to the parent
            else if (curNode.Intendation == intend)
                node = new YamlLight(curNode.Parent, intend);
                // same level as parent or less
            else
            {
                YamlLight parent = curNode.Parent;
                while (parent != null && parent.Intendation > intend)
                    parent = parent.Parent;
                if (parent == null)
                    throw new InvalidDataException("Failed to find parent node on line " + lineNumber);
                node = new YamlLight(parent.Parent, intend);
            }

            return node;
        }

        protected void ParseData(string line, int offset, int lineNumber)
        {
            int pos = line.IndexOf(':', offset);
            if (pos == -1)
                throw new InvalidDataException("Failed to find value on line " + lineNumber);

            Name = line.Substring(offset, pos - offset);
            if (pos < line.Length-1)
            Value = line.Substring(pos + 1).Trim();
        }

        static int GetIntend(string line)
        {
            for (int i = 0; i < line.Length; ++i)
            {
                if (!char.IsWhiteSpace(line[i]))
                    return i;
            }

            return -1;
        }

        IEnumerator<YamlLight> IEnumerable<YamlLight>.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }
}
