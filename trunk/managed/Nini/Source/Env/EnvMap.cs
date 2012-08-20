using System;
using Nini.Util;

namespace Nini.Env
{
    // This gets created by the EnvConfigSource and is the "document" side
    // that holds the keys that we will query the environment for the values.
    // And, it will also hold the key:value pairs that we can set in the
    // environmant space as well.
    //
    // We don't need sections here, just a single map
    // Implemented as a thread-safe singleton to ensure we end up with one
    // authoratitive environment map for the application
    //
    // We will look to synchronize all lists in the shell env, EnvItems and here
    // *** It looks like the events should be handled at the application level
    // *** So, we won't try doing it here for the time-being
    //
    //
    public sealed class EnvMap
    {

        #region private variables
        private static volatile EnvMap instance;
        private static object syncRoot = new Object();
        private OrderedList envList;
        private bool synchronized = false;
        private EnvAdapter env = null;
        #endregion

        #region constructors
        /// <summary>
        /// constructor
        /// </summary>
        private EnvMap()
        {
            env = new EnvAdapter();
        }
        #endregion
        #region public properties
        /// <summary>
        /// Synchronize with source (shell environment) and EnvConfig
        /// </summary>
        public bool Synchronized {

            get { return synchronized; }

            set { synchronized = value; }
        }
        /// <summary>
        /// Get/Set key - value pairs in our OrderedList
        /// </summary>
        /// <param name="key">
        /// A <see cref="System.String"/>
        /// </param>
        public string this[string key]
        {
            get {
                if(envList.Contains(key))
                {
                    lock (syncRoot) {
                        return envList[key].ToString();
                    }
                }
                else
                {
                    return null;
                }
            }

            set {

                lock (syncRoot) {

                    envList[key] = value;
                }
            }
        }
        /// <summary>
        /// Get our list of shell environment variables;
        /// </summary>
        public OrderedList EnvList
        {
            get {
                lock (syncRoot) {
                    return envList;
                }
            }
        }
        #endregion
        #region public methods
        /// <summary>
        /// Returns the singleton EnvMap instance
        /// </summary>
        public static EnvMap Instance {
            get {

                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new EnvMap();
                            instance.envList = new OrderedList();
                        }
                    }
                }

                return instance;
            }
        }
        /// <summary>
        /// Return a shell environment variable
        /// </summary>
        /// <param name="index">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="EnvItem"/>
        /// </returns>
        public EnvItem GetItem (int index)
        {
            return (EnvItem) EnvList[index];
        }
        /// <summary>
        /// Add a key:val to the map with the defaultValue being assigned if
        /// the environment variable is empty
        /// </summary>
        /// <param name="envar">
        /// A <see cref="System.String"/>
        /// </param>
        public void Add(string key, string defaultValue)
        {
            EnvItem item = new EnvItem(key, env.Get(key, defaultValue), "" );
            envList.Add(key, item);
        }

        public void Save()
        {

        }
        #endregion
        #region private methods
        #endregion
    }
}

