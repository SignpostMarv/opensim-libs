using System;
using System.Collections;
using System.Collections.Generic;

namespace HttpServer
{
    /// <summary>
    /// represents a http input item. Each item can have multiple sub items, a sub item
    /// is made in a html form by using square brackets
    /// </summary>
    /// <example>
    ///   // <input type="text" name="user[FirstName]" value="jonas" /> becomes:
    ///   Console.WriteLine("Value: {0}", form["user"]["FirstName"].Value);
    /// </example>
    /// <remarks>
    /// All names in a form SHOULD be in lowercase.
    /// </remarks>
    public class HttpInputItem : HttpInputBase
    {
        public static readonly HttpInputItem Empty = new HttpInputItem(string.Empty, true);
        private readonly IDictionary<string, HttpInputItem> _items = new Dictionary<string, HttpInputItem>();
        private readonly IList<string> _values = new List<string>();
        private string _name;
        private readonly bool _ignoreChanges = false;

        public HttpInputItem(string name, string value)
        {
            Name = name;
            Add(value);
        }

        private HttpInputItem(string name, bool ignore)
        {
            Name = name;
            _ignoreChanges = ignore;
        }

        /// <summary>
        /// Number of values
        /// </summary>
        public int Count
        {
            get { return _values.Count; }
        }

        /// <summary>
        /// Get a sub item
        /// </summary>
        /// <param name="name">name in lower case.</param>
        /// <returns>HttpInputItem.Empty if no item was found.</returns>
        public HttpInputItem this[string name]
        {
            get
            {
                if (_items.ContainsKey(name))
                    return _items[name];
                else
                    return null;
            }
        }

        /// <summary>
        /// Name of item (in lower case).
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Returns the first value, or null if no value exist.
        /// </summary>
        public string Value
        {
            get
            {
                if (_values.Count == 0)
                    return null;
                else
                    return _values[0];
            }
        }

        /// <summary>
        /// Returns the list with values.
        /// todo: Return a readonly collection
        /// </summary>
        public IList<string> Values
        {
            get { return _values; }
        }


        /// <summary>
        /// Add another value to this item
        /// </summary>
        /// <param name="value"></param>
        public void Add(string value)
        {
            if (value == null)
                return;
            if (_ignoreChanges)
                throw new InvalidOperationException("Cannot add stuff to HttpInput.Empty.");

            _values.Add(value);
        }

        /// <summary>
        /// checks if a subitem exists (and has a value).
        /// </summary>
        /// <param name="name">name in lower case</param>
        /// <returns>true if the subitem exists and has a value; otherwise false.</returns>
        public bool Contains(string name)
        {
            return _items.ContainsKey(name) && _items[name].Value != null;
        }

        public override string ToString()
        {
            return ToString(string.Empty);
        }

        public string ToString(string prefix)
        {
            string name;
            if (string.IsNullOrEmpty(prefix))
                name = Name;
            else
                name = prefix + "[" + Name + "]";

            string temp = name;
            if (_values.Count > 0)
            {
                temp += " = ";
                foreach (string value in _values)
                    temp += value + ", ";
                temp = temp.Remove(temp.Length - 2, 2);
            }
            temp += Environment.NewLine;

            foreach (KeyValuePair<string, HttpInputItem> item in _items)
                temp += item.Value.ToString(name);

            return temp;
        }

        #region HttpInputBase Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">name in lower case</param>
        /// <returns></returns>
        HttpInputItem HttpInputBase.this[string name]
        {
            get
            {
                if (_items.ContainsKey(name))
                    return _items[name];
                else
                    return Empty;
            }
        }

        /// <summary>
        /// Add a sub item
        /// </summary>
        /// <param name="name">Can contain array formatting, the item is then parsed and added in multiple levels</param>
        /// <param name="value"></param>
        public void Add(string name, string value)
        {
            if (name == null && value != null)
                throw new ArgumentNullException("name");
            if (name == null)
                return;
            if (_ignoreChanges)
                throw new InvalidOperationException("Cannot add stuff to HttpInput.Empty.");

            if (name.Contains("["))
            {
                HttpInputItem item = HttpInput.ParseItem(name, value);

                // Add the value to an existing sub item
                if (_items.ContainsKey(item.Name))
                    _items[item.Name].Add(item.Value);
                else
                    _items.Add(item.Name, item);
            }
            else
            {
                if (_items.ContainsKey(name))
                    _items[name].Add(value);
                else
                    _items.Add(name, new HttpInputItem(name, value));
            }
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,HttpInputItem>> Members

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
}