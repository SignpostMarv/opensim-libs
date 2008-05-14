using System;
using System.Collections.Generic;
using HttpServer;
using HttpServer.Sessions;
using Tiny;
using Tiny.Data;

namespace TestApp.Sessions
{
    /// <summary>
    /// This session store saves sessions into the database by using
    /// the Tiny ORM Layer
    /// </summary>
    public class TinySessionStore : HttpSessionStore
    {
        private int _expireTime = 20;
        private readonly DataManager _mgr;
        private readonly Dictionary<string, MemorySession> _cachedSessions = new Dictionary<string, MemorySession>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mgr">Datamanager used to talk to the database.</param>
        public TinySessionStore(DataManager mgr)
        {
            _mgr = mgr;
        }

        #region HttpSessionStore Members

        /// <summary>
        /// Load a session from the store
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>null if session is not found.</returns>
        public HttpSession this[string sessionId]
        {
            get { return Load(sessionId); }
        }

        /// <summary>
        /// Number of minutes before a session expires.
        /// Default is 20 minutes.
        /// </summary>
        public int ExpireTime
        {
            get {return  _expireTime; }
            set { _expireTime = value; }
        }

        /// <summary>
        /// Creates a new http session
        /// </summary>
        /// <returns>A HttpSession object</returns>
        public HttpSession Create()
        {
            return new TinySession(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Creates a new http session with a specific id
        /// </summary>
        /// <returns>A HttpSession object.</returns>
        public HttpSession Create(string id)
        {
            return new TinySession(id);
        }

        /// <summary>
        /// Load an existing session.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>A session if found; otherwise null.</returns>
        public HttpSession Load(string sessionId)
        {
            lock (_cachedSessions)
            {
                if (_cachedSessions.ContainsKey(sessionId))
                    return _cachedSessions[sessionId];
            }

                    TinySession session = LoadFromDb(sessionId);
                    if (session == null)
                        return null;

            lock (_cachedSessions)
                _cachedSessions.Add(session.Id, session);
            return session;
            
        }

        /// <summary>
        /// Save an updated session to the store.
        /// </summary>
        /// <param name="session"></param>
        /// <exception cref="ArgumentException">If Id property have not been specified.</exception>
        public void Save(HttpSession session)
        {
            //todo: catch exception
            _mgr.Update(session);
        }

        /// <summary>
        /// We use the flyweight pattern which reuses small objects
        /// instead of creating new each time.
        /// </summary>
        /// <param name="session">Empty (unused) session that should be reused next time Create is called.</param>
        public void AddUnused(HttpSession session)
        {
            // Let's implement this later. 
            session.Dispose();
        }

        /// <summary>
        /// Remove expired sessions
        /// </summary>
        public void Cleanup()
        {
            foreach (KeyValuePair<string, MemorySession> pair in _cachedSessions)
            {
                if (pair.Value.Accessed.AddMinutes(_expireTime) < DateTime.Now)
                {
                    _mgr.Remove(pair.Key);
                    _cachedSessions.Remove(pair.Key);
                    break; // foreach do not work when list ahve been modified.
                }
            }
        }

        #endregion

        private TinySession LoadFromDb(string id)
        {
            try
            {
                return _mgr.Fetch<TinySession>(id);
            }
            catch (TinyException)
            {
                return null;
            }
        }
    }
}
