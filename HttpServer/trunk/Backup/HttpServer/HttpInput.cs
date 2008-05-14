using System;
using System.Collections;
using System.Collections.Generic;

namespace HttpServer
{
    /// <summary>
    /// Replacement of HttpInput, contains
    /// information entered in either query string or
    /// content body.
    /// </summary>
    public class HttpInput : HttpInputBase
    {
        public static readonly HttpInput Empty = new HttpInput("Empty", true);
        private readonly IDictionary<string, HttpInputItem> _items = new Dictionary<string, HttpInputItem>();
        private string _name;
        private readonly bool _ignoreChanges = false;

        /// <summary>
        /// Create a new form
        /// </summary>
        /// <param name="name"></param>
        public HttpInput(string name)
        {
            Name = name;
        }

        private HttpInput(string name, bool ignoreChanges)
        {
            _name = name;
            _ignoreChanges = ignoreChanges;
        }

        /// <summary>
        /// Name as lower case
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Add a new element. Form array elements are parsed
        /// and added in a correct hierachy.
        /// </summary>
        /// <param name="name">Name is converted to lower case.</param>
        /// <param name="value"></param>
        public void Add(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (_ignoreChanges)
                throw new InvalidOperationException("Cannot add stuff to HttpInput.Empty.");

            // Check if it's a sub item.
            // we can have multiple levels of sub items as in user[extension[id]] => user -> extension -> id
            int pos = name.IndexOf('[');
            if (pos != -1)
            {
                string name1 = name.Substring(0, pos);
                string name2 = ExtractOne(name);
                if (!_items.ContainsKey(name1))
                    _items.Add(name1, new HttpInputItem(name1, null));
                _items[name1].Add(name2, value);
            }
            else
            {
                if (_items.ContainsKey(name))
                    _items[name].Add(value);
                else
                    _items.Add(name, new HttpInputItem(name, value));
            }
        }

        /// <summary>
        /// Get a form item.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns HttpInputItem.Empty if item was not found.</returns>
        public HttpInputItem this[string name]
        {
            get
            {
                if (_items.ContainsKey(name))
                    return _items[name];
                else
                    return HttpInputItem.Empty;
            }
        }

        public bool Contains(string name)
        {
            return _items.ContainsKey(name) && _items[name].Value != null;
        }

        /// <summary>
        /// Parses an item and returns it.
        /// This function is primarly used to parse array items as in user[name].
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static HttpInputItem ParseItem(string name, string value)
        {
            HttpInputItem item;

            // Check if it's a sub item.
            // we can have multiple levels of sub items as in user[extension[id]]] => user -> extension -> id
            int pos = name.IndexOf('[');
            if (pos != -1)
            {
                string name1 = name.Substring(0, pos);
                string name2 = ExtractOne(name);
                item = new HttpInputItem(name1, null);
                item.Add(name2, value);
            }
            else
                item = new HttpInputItem(name, value);

            return item;
        }

        public override string ToString()
        {
            string temp = string.Empty;
            foreach (KeyValuePair<string, HttpInputItem> item in _items)
                temp += item.Value.ToString(Name);
            return temp;
        }

        /// <summary>
        /// Extracts one parameter from an array
        /// </summary>
        /// <param name="value">Containing the string array</param>
        /// <returns>All but the first value</returns>
        /// <example>
        /// string test1 = ExtractOne("system[user][extension][id]");
        /// string test2 = ExtractOne(test1);
        /// string test3 = ExtractOne(test2);
        /// // test1 = user[extension][id]
        /// // test2 = extension[id]
        /// // test3 = id
        /// </example>
        public static string ExtractOne(string value)
        {
            int pos = value.IndexOf('[');
            if (pos != -1)
            {
                ++pos;
                int gotMore = value.IndexOf('[', pos + 1);
                if (gotMore != -1)
                    value = value.Substring(pos, gotMore - pos - 1) + value.Substring(gotMore);
                else
                    value = value.Substring(pos, value.Length - pos - 1);
            }
            return value;
        }

        #region IEnumerable<KeyValuePair<string,HttpInputBase>> Members

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        IEnumerator<KeyValuePair<string, HttpInputItem>> IEnumerable<KeyValuePair<string, HttpInputItem>>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion
    }

    public interface HttpInputBase : IEnumerable<KeyValuePair<string, HttpInputItem>>
    {
        void Add(string name, string value);

        HttpInputItem this[string name]
        { get; }

        bool Contains(string name);
    }
}
