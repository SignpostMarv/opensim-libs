using System;
namespace Nini.Env
{
    public class EnvAdapter
    {
        public EnvAdapter()
        {
        }
        /// <summary>
        /// Get shell environment variable
        /// </summary>
        /// <param name="key">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="defaultValue">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public string Get (string key, string defaultValue)
        {
            string val = Environment.GetEnvironmentVariable(key);
            if(val == null)
            {
                this.Set(key, defaultValue);
                return defaultValue;
            }
            return val;
        }
        /// <summary>
        /// Set shell environment variable
        /// </summary>
        /// <param name="key">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="val">
        /// A <see cref="System.String"/>
        /// </param>
        public void Set (string key, string val)
        {
            Environment.SetEnvironmentVariable(key, val);
        }
    }
}

