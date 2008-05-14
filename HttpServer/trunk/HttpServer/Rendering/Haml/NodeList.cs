using System;
using System.Collections.Generic;
using HttpServer.Rendering.Haml.Nodes;

namespace HttpServer.Rendering.Haml
{
    public class NodeList
    {
        private readonly List<Node> _nodes = new List<Node>();

        public Node CreateNode(string word, Node parent)
        {
            foreach (Node node in _nodes)
            {
                if (node.CanHandle(word, parent == null))
                    return (Node) Activator.CreateInstance(node.GetType(), new object[]{parent});
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <param name="firstNode">first node on line</param>
        /// <returns></returns>
        public Node GetPrototype(string word, bool firstNode)
        {
            foreach (Node node in _nodes)
            {
                if (node.CanHandle(word, firstNode))
                    return node;
            }

            return null;            
        }

        public void Add(Node node)
        {
            //todo: Replace types.
            _nodes.Add(node);
        }
    }
}
