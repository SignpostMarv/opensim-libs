using System;
using System.Runtime.Serialization;

namespace HttpServer
{
    /// <summary>
    /// Interface for sessions
    /// </summary>
    public interface HttpSession : IDisposable
    {
        /// <summary>
        /// Session id
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Should 
        /// </summary>
        /// <param name="name">Name of the session variable</param>
        /// <returns>null if it's not set</returns>
        /// <exception cref="SerializationException">If the object cant be serialized.</exception>
        object this[string name] { get; set; }

        /// <summary>
        /// When the session was last accessed.
        /// This property is touched by the http server each time the
        /// session is requested.
        /// </summary>
        DateTime Accessed { get; set; }

        /// <summary>
        /// Number of session variables.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Remove everything from the session
        /// </summary>
        void Clear();
    }
}