using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer.Language.Yaml.Tags
{
    class Tag
    {
        private string _name;
        private string _kind;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }
    }
}
