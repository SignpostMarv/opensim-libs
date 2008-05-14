using HttpServer.Sessions;

namespace TestApp.Sessions
{
    /// <summary>
    /// Small hack since the ORM layer just looks for the mapping file
    /// in the same assembly as the class was loaded from.
    /// </summary>
    public class TinySession : MemorySession
    {
        public TinySession(string id) : base(id)
        {
            
        }
    }
}
