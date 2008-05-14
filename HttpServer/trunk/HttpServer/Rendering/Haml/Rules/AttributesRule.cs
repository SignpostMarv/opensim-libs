using System;
using HttpServer.Rendering;

namespace HttpServer.Rendering.Haml.Rules
{
    public class AttributesRule : Rule
    {
        public override bool IsMultiLine(LineInfo line)
        {
            // hack to dont include code
            // a more proper way would have bene to scan after each tag
            for (int i = 0; i < line.Data.Length; ++i)
            {
                char ch = line.Data[i];
                if (!char.IsWhiteSpace(ch))
                {
                    if (ch != '#' && ch != '%')
                        return false;
                }
            }

            bool inQuote = false;
            bool inAttribute = false;
            foreach (char ch in line.Data)
            {
                if (ch == '"')
                    inQuote = !inQuote;
                else if (ch == '{' && !inQuote)
                {
                    if (inAttribute)
                        throw new CodeGeneratorException(line.LineNumber,
                            "Found another start of attributes, but no close tag. Have you forgot one '}'?");
                    inAttribute = true;
                }
                else if (ch == '}' && !inQuote)
                    inAttribute = false;
            }

            if (inAttribute)
            {
                Console.WriteLine("Attribute is not closed, setting unfinished rule");
                line.UnfinishedRule = this;
            }

            return inAttribute;
        }


    }
}
