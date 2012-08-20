
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Nini.Util;
using Nini.Env;

namespace Nini.Config
{
    // This needs to use the events to make changes to the shell environment
    // if a key-pair changes in the EnvConfig occurs
    //
    // This class knows how to load and save the environment variables. It is our
    // "document". This is the system facing side of our implementation that will

    //
    public class EnvConfigSource : ConfigSourceBase
    {
        #region private variables
        EnvMap envmap = EnvMap.Instance;
        bool caseSensitive = true;
        #endregion

        #region constructors
        // We will need to create an EnvMap and populate it with the
        // keys to the environmant variable we are interested in using
        //
        // Then it will load them, or set a default
        //
        public EnvConfigSource()
        {
			
        }
        #endregion
        #region public properties
        #endregion
        #region public methods
        public void AddEnv(string key, string defaultValue)
        {
            envmap.Add(key, defaultValue);
        }
        // Can use this to set environment variables in the shell
        //
        public override void Save() {
            envmap.Save();
            base.Save();
        }
        public void LoadEnv ()
        {
            this.Configs.Clear ();
            Load ();
        }

		/// <summary>
		/// The case sensitive flag
		/// </summary>
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { caseSensitive = value; }
        }
        #endregion
        #region private methods
        /// <summary>
        /// Load the EnvMap key:val pairs into our application facing side
        /// </summary>
        private void Load ()
        {
            // TODO: After initial testing, look at adding "Sections" for management
            EnvConfig config = new EnvConfig("Environment", this);
            this.Configs.Add (config);
            EnvItem item = null;

            for (int j = 0; j < envmap.EnvList.Count; j++)
            {
                item = envmap.GetItem(j);
                if  (item.Name != null) {
                        config.Add (item.Name, item.Value);
                 }
//                if ( null != item.Value)
//                {
//                    config.Set(item.Name, item.Value);
//                }
            }
        }
        #endregion
    }
}