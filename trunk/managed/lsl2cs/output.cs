// used for testing the compiler

using System;
using System.IO;
using Tools;
using OpenSim.Region.ScriptEngine.Shared.CodeTools;

public class ex
{
    private static int m_indent = 0;

    public static void Main(string[] argv) {
        Parser p = new LSLSyntax();
        StreamReader s = new StreamReader(argv[0]);
        SYMBOL ast = p.Parse(s);
        if (ast!=null)
        {
            if (1 < argv.Length && "-t" == argv[1])
            {
                printThisTree(new LSL2CSCodeTransformer(ast).Transform());
            }
            else
            {
                LSL2CSCodeTransformer ct = new LSL2CSCodeTransformer(ast);
                //CSCodeGenerator cscg = new CSCodeGenerator(ct.Transform(), Console.OpenStandardOutput());
                CSCodeGenerator cscg = new CSCodeGenerator(ct.Transform());
                //cscg.Generate();
                Console.Write(cscg.Generate());
            }
        }

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
