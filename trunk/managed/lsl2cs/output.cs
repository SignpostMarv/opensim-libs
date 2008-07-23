// used for testing the compiler

using System;
using System.IO;
using Tools;
using OpenSim.Region.ScriptEngine.Shared.CodeTools;

public class ex
{
    private static int m_indent = 0;

    public static void Main(string[] argv) {
        StreamReader s = new StreamReader(argv[0]);
        string source = s.ReadToEnd();

        CSCodeGenerator cscg = new CSCodeGenerator();
        string output = cscg.Convert(source);

        if (1 < argv.Length && "-t" == argv[1])
        {
            Parser p = new LSLSyntax();
            LSL2CSCodeTransformer codeTransformer = new LSL2CSCodeTransformer(p.Parse(source));
            SYMBOL ast = codeTransformer.Transform();
            printThisTree(ast);
        }
        else
            Console.Write(output);
    }

    public static void printThisTree(SYMBOL ast)
    {
        for (int i = 0; i < m_indent; i++)
            Console.Write("  ");
        Console.WriteLine(ast.ToString());

        m_indent++;
        foreach (SYMBOL s in ast.kids)
            printThisTree(s);
        m_indent--;
    }
}
