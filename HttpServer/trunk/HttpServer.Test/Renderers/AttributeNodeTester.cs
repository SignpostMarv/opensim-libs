using System;
using System.Collections.Generic;
using System.Text;
using HttpServer.Rendering.Haml;
using HttpServer.Rendering.Haml.Nodes;
using NUnit.Framework;

namespace HttpServer.Test.Renderers
{
    [TestFixture]
    public class AttributeNodeTester
    {

        [Test]
        public void Test()
        {
            LineInfo line = new LineInfo(1, string.Empty);
            line.Set("%input{ type=\"checkbox\", value=testCase.Id, name=\"case\", class=lastCat.Replace(' ', '-')}", 0,
                     0);

            TagNode tagNode = new TagNode(null);
            NodeList nodes = new NodeList();
            AttributeNode node = new AttributeNode(tagNode);
            nodes.Add(node);
            nodes.Add(tagNode);

            int offset = 6;
            AttributeNode myNode  = (AttributeNode)node.Parse(nodes, tagNode, line, ref offset);
            Assert.AreEqual("\"checkbox\"", myNode.Attributes["type"]);
            Assert.AreEqual("testCase.Id", myNode.Attributes["value"]);
            Assert.AreEqual("\"case\"", myNode.Attributes["name"]);
            Assert.AreEqual("lastCat.Replace(' ', '-')", myNode.Attributes["class"]);
        }
    }
}
