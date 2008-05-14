using System;
using System.Collections.Generic;
using HttpServer.Sessions;

namespace HttpServer.Sessions
{
    /// <summary>
    /// Session store using memory for each session.
    /// 
    /// todo: Remove expired sessions. We need to code a Scheduler service
    /// in tiny library and use it.
    /// </summary>
    public class MemorySessionStore : HttpSessionStore
    {
        private readonly IDictionary<string, HttpSession> _sessions = new Dictionary<string, HttpSession>();
        private readonly Queue<HttpSession> _unusedSessions = new Queue<HttpSession>();
        private int _expireTime = 20;

        #region HttpSessionStore Members

        /// <summary>
        /// Load a session from the store
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>null if session is not found.</returns>
        public HttpSession this[string sessionId]
        {
            get
            {
                lock (_sessions)
                {
                    if (!_sessions.ContainsKey(sessionId))
                        return null;
                    else
                        return _sessions[sessionId];
                }
            }
        }

        /// <summary>
        /// Creates a new http session
        /// </summary>
        /// <returns></returns>
        public HttpSession Create()
        {
            return Create(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Creates a new http session with a specific id
        /// </summary>
        /// <returns></returns>
        public HttpSession Create(string id)
        {
            lock (_unusedSessions)
            {
                if (_unusedSessions.Count > 0)
                {
                    MemorySession session = _unusedSessions.Dequeue() as MemorySession;
                    if (session != null)
                    {
                        session.SetId(id);
                        return session;
                    }
                }
            }

            return new MemorySession(id);
        }

        /// <summary>
        /// Load an existing session.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public HttpSession Load(string sessionId)
        {
            if (_sessions.ContainsKey(sessionId))
                return _sessions[sessionId];
            else
                return null;
        }

        /// <summary>
        /// Save an updated session to the store.
        /// </summary>
        /// <param name="session"></param>
        public void Save(HttpSession session)
        {
            lock (_sessions)
            {
                if (_sessions.ContainsKey(session.Id))
                    _sessions[session.Id] = session;
                else
                    _sessions.Add(session.Id, session);
            }
        }

        /// <summary>
        /// We use the flyweight pattern which reuses small objects
        /// instead of creating new each time.
        /// </summary>
        /// <param name="session">Empty (unused) session that should be reused next time Create is called.</param>
        public void AddUnused(HttpSession session)
        {
            lock (_unusedSessions)
                _unusedSessions.Enqueue(session);
        }

        /// <summary>
        /// Remove expired sessions
        /// </summary>
        public void Cleanup()
        {
            lock (_sessions)
            {
                foreach (KeyValuePair<string, HttpSession> pair in _sessions)
                {
                    // don't reuse used sessions since they may be in use when we remove them from the list.
                    TimeSpan liveTime = DateTime.Now.Subtract(pair.Value.Accessed);
                    if (liveTime.Minutes > _expireTime)
                    {
                        _sessions.Remove(pair.Key);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Number of minutes before a session expires.
        /// Default is 20 minutes.
        /// </summary>
        public int ExpireTime
        {
            get { return _expireTime; }
            set { _expireTime = value; }
        }

        #endregion


    }
}