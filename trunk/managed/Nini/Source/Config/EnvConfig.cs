using System;

namespace Nini.Config
{
    // This is our application facing implementation to hold the list
    // of key:value pairs in ConfigBase.keys. Our Application will use this to
    // access/manipulate the pairs
    //
    public class EnvConfig : ConfigBase
    {
        #region Private variables
        EnvConfigSource parent = null;
        #endregion
        #region constructors
        public EnvConfig(string name, IConfigSource source)
            :base(name, source)
        {
            parent = (EnvConfigSource)source;
        }
        #endregion
        #region public methods
        public override void Set (string key, object value)
        {
            if (!parent.CaseSensitive) {
                key = CaseInsensitiveKeyName (key);
            }

            base.Set (key, value);
        }
        #endregion
        #region Private methods
        /// <summary>
        /// Returns the key name if the case insensitivity is turned on.  
        /// </summary>
        private string CaseInsensitiveKeyName (string key)
        {
            string result = null;
            string lowerKey = key.ToLower ();

            foreach (string currentKey in keys.Keys)
            {
                if (currentKey.ToLower () == lowerKey) {
                    result = currentKey;
                    break;
                }
            }
            return (result == null) ? key : result;
        }
        #endregion
    }
}

