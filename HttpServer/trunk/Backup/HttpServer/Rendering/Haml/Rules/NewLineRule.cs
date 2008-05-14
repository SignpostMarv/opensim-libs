using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer.Rendering.Haml.Rules
{
    public class NewLineRule : Rule
    {
        public override bool IsMultiLine(LineInfo line)
        {
            string trimmed = line.Data.TrimEnd();
            if (trimmed.Length == 0)
                return false;

            if (trimmed[trimmed.Length - 1] == '|')
            {
                line.TrimRight(1);
                return true;
            }

            return false;
        }
    }
}
