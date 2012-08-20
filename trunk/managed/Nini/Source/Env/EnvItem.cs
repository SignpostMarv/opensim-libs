using System;


namespace Nini.Env
{
    public class EnvItem {

        #region Private variables
        string envName = "";
        string envValue = "";
        string envComment = null;
        #endregion

        #region Public properties
        public string Value
        {
            get { return envValue; }
            set { envValue = value; }
        }

        public string Name
        {
            get { return envName; }
        }
        #endregion

        internal protected EnvItem (string name, string val, string comment)
        {
            envName = name;
            envValue = val;
            envComment = comment;
        }
    }
}
