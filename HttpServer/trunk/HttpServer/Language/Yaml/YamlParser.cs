using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HttpServer.Language.Yaml.Tags;

namespace HttpServer.Language.Yaml
{
    public class YamlParser
    {
        public void Parse(string fileName)
        {
            string text = File.ReadAllText(fileName);
            StringReader reader = new StringReader(text);

            Node mother = new Node(-1);

            int lastIntend = 0;
            string line = reader.ReadLine();
            Node node = mother;
            int lineNumber = 1;
            while (line != null)
            {
                int intend;
                int offset = GetIntendation(line, out intend);
                if (intend > node.Intend +1)
                    throw new InvalidOperationException("Too large intendation step on line " + lineNumber);

                if (intend > node.Intend)
                    node.Children.AddLast(new Node(intend, node));
                else
                    node.Parent.Children.AddLast(new Node(intend, node.Parent));



                line = reader.ReadLine();
                ++lineNumber;
            }
        }

        private int GetIntendation(string line, out int steps)
        {
            steps = 0;

            bool ignoreNextSpace = false;
            for (int i = 0; i < line.Length; ++i)
            {
                char ch = line[i];
                char nextCh = (i < line.Length - 1) ? line[i + 1] : char.MinValue;

                if (char.IsWhiteSpace(ch))
                {
                    if (ignoreNextSpace)
                    {
                        ignoreNextSpace = false;
                        continue;
                    }
                    else if (ch == ' ')
                    {
                        if (nextCh == ' ')
                            ignoreNextSpace = true;

                        ++steps; // two spaces = one intend.
                    }
                    else
                        ++steps; // tab
                }
                if (!char.IsWhiteSpace(line[i]))
                    return i;
            }

            return line.Length;
        }
    }
}