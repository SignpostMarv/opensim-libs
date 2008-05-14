namespace HttpServer.Rendering.Haml.Rules
{
    public abstract class Rule
    {
        public abstract bool IsMultiLine(LineInfo line);
    }
}