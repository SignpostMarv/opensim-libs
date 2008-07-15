using System;
using System.Collections.Generic;
using Tools;

namespace OpenSim.Region.ScriptEngine.Shared.CodeTools
{
    public class LSL2CSCodeTransformer
    {
        private SYMBOL m_astRoot = null;
        private static Dictionary<string, string> m_datatypeLSL2OpenSim = new Dictionary<string, string>();

        /// <summary>
        /// Pass the new CodeTranformer an abstract syntax tree.
        /// </summary>
        /// <param name="astRoot">The root node of the AST.</param>
        public LSL2CSCodeTransformer(SYMBOL astRoot)
        {
            m_astRoot = astRoot;

            // let's populate the dictionary
            m_datatypeLSL2OpenSim.Add("integer", "LSL_Types.LSLInteger");
            m_datatypeLSL2OpenSim.Add("float", "LSL_Types.LSLFloat");
            //m_datatypeLSL2OpenSim.Add("key", "LSL_Types.key"); // key doesn't seem to be used
            m_datatypeLSL2OpenSim.Add("key", "LSL_Types.LSLString");
            m_datatypeLSL2OpenSim.Add("string", "LSL_Types.LSLString");
            m_datatypeLSL2OpenSim.Add("vector", "LSL_Types.Vector3");
            m_datatypeLSL2OpenSim.Add("rotation", "LSL_Types.Quaternion");
            m_datatypeLSL2OpenSim.Add("list", "LSL_Types.list");
        }

        /// <summary>
        /// Transform the code in the AST we have.
        /// </summary>
        /// <returns>The root node of the transformed AST</returns>
        public SYMBOL Transform()
        {
            foreach (SYMBOL s in m_astRoot.kids)
                TransformNode(s);

            return m_astRoot;
        }

        /// <summary>
        /// Recursively called to transform each type of node. Will transform this
        /// node, then all it's children.
        /// </summary>
        /// <param name="s">The current node to transform.</param>
        private void TransformNode(SYMBOL s)
        {
            // make sure to put type lower in the inheritance hierarchy first
            // ie: since IdentConstant and StringConstant inherit from Constant,
            // put IdentConstant and StringConstant before Constant
            if (s is Declaration)
                ((Declaration) s).Datatype = m_datatypeLSL2OpenSim[((Declaration) s).Datatype];
            else if (s is Constant)
                ((Constant) s).Type = m_datatypeLSL2OpenSim[((Constant) s).Type];
            else if (s is TypecastExpression)
                ((TypecastExpression) s).TypecastType = m_datatypeLSL2OpenSim[((TypecastExpression) s).TypecastType];
            else if (s is GlobalFunctionDefinition && "void" != ((GlobalFunctionDefinition) s).ReturnType) // we don't need to translate "void"
                ((GlobalFunctionDefinition) s).ReturnType = m_datatypeLSL2OpenSim[((GlobalFunctionDefinition) s).ReturnType];

            for (int i = 0; i < s.kids.Count; i++)
            {
                if (!(s is Assignment || s is ArgumentDeclarationList) && s.kids[i] is Declaration)
                    AddImplicitInitialization(s, i);

                TransformNode((SYMBOL) s.kids[i]);
            }
        }

        /// <summary>
        /// Replaces an instance of the node at s.kids[didx] with an assignment
        /// node. The assignment node has the Declaration node on the left hand
        /// side and a default initializer on the right hand side.
        /// </summary>
        /// <param name="s">
        /// The node containing the Declaration node that needs replacing.
        /// </param>
        /// <param name="didx">Index of the Declaration node to replace.</param>
        private void AddImplicitInitialization(SYMBOL s, int didx)
        {
            // We take the kids for a while to play with them.
            int sKidSize = s.kids.Count;
            object [] sKids = new object[sKidSize];
            for (int i = 0; i < sKidSize; i++)
                sKids[i] = s.kids.Pop();

            // The child to be changed.
            Declaration currentDeclaration = (Declaration) sKids[didx];

            // We need an assignment node.
            Assignment newAssignment = new Assignment(currentDeclaration.yyps,
                                                      currentDeclaration,
                                                      GetZeroConstant(currentDeclaration.yyps, currentDeclaration.Datatype),
                                                      "=");
            sKids[didx] = newAssignment;

            // Put the kids back where they belong.
            for (int i = 0; i < sKidSize; i++)
                s.kids.Add(sKids[i]);
        }

        /// <summary>
        /// Generates the node structure required to generate a default
        /// initialization.
        /// </summary>
        /// <param name="p">
        /// Tools.Parser instance to use when instantiating nodes.
        /// </param>
        /// <param name="constantType">String describing the datatype.</param>
        /// <returns>
        /// A SYMBOL node conaining the appropriate structure for intializing a
        /// constantType.
        /// </returns>
        private SYMBOL GetZeroConstant(Parser p, string constantType)
        {
            switch (constantType)
            {
            case "integer":
                return new Constant(p, constantType, "0");
            case "float":
                return new Constant(p, constantType, "0.0");
            case "string":
            case "key":
                return new Constant(p, constantType, "");
            case "list":
                ArgumentList al = new ArgumentList(p);
                return new ListConstant(p, al);
            case "vector":
                Constant vca = new Constant(p, "float", "0.0");
                Constant vcb = new Constant(p, "float", "0.0");
                Constant vcc = new Constant(p, "float", "0.0");
                ConstantExpression vcea = new ConstantExpression(p, vca);
                ConstantExpression vceb = new ConstantExpression(p, vcb);
                ConstantExpression vcec = new ConstantExpression(p, vcc);
                return new VectorConstant(p, vcea, vceb, vcec);
            case "rotation":
                Constant rca = new Constant(p, "float", "0.0");
                Constant rcb = new Constant(p, "float", "0.0");
                Constant rcc = new Constant(p, "float", "0.0");
                Constant rcd = new Constant(p, "float", "0.0");
                ConstantExpression rcea = new ConstantExpression(p, rca);
                ConstantExpression rceb = new ConstantExpression(p, rcb);
                ConstantExpression rcec = new ConstantExpression(p, rcc);
                ConstantExpression rced = new ConstantExpression(p, rcd);
                return new RotationConstant(p, rcea, rceb, rcec, rced);
            default:
                return null; // this will probably break stuff
            }
        }
    }
}
