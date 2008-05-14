namespace HttpServer.Rendering
{
    /// <summary>
    /// Interface for dynamically generated templates.
    /// </summary>
    /// <seealso cref="TemplateManager"/>
    public interface TinyTemplate
    {
        string Invoke(object[] args);
    }
}
