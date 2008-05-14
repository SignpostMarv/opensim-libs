using System;
using System.Collections;
using System.Collections.Generic;

namespace HttpServer
{
    /// <summary>
    /// Returns item either from a form or a query string (checks them in that order)
    /// </summary>
    public class HttpParam : HttpInputBase
    {
        public static readonly HttpParam Empty = new HttpParam(HttpInput.Empty, HttpInput.Empty);

        private HttpInputBase _form;
        private HttpInputBase _query;

        public HttpParam(HttpInputBase form, HttpInputBase query)
        {
            _form = form;
            _query = query;
        }

        #region HttpInputBase Members

        /// <summary>
        /// The add method is not availible for HttpParam
        /// since HttpParam checks both Request.Form and Request.QueryString
        /// </summary>
        /// <param name="name">name identifying the value</param>
        /// <param name="value">value to add</param>
        /// <exception cref="NotImplementedException"></exception>
        [Obsolete("Not implemented for HttpParam")]
        public void Add(string name, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the form or querystring has the specified value
        /// </summary>
        /// <param name="name">Name, case sensitive</param>
        /// <returns>true if found; otherwise false.</returns>
        public bool Contains(string name)
        {
            return _form.Contains(name) || _query.Contains(name);
        }

        /// <summary>
        /// Fetch an item from the form or querystring (in that order).
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Item if found; otherwise HttpInputItem.Empty</returns>
        public HttpInputItem this[string name]
        {
            get 
            {
                if (_form[name] != HttpInputItem.Empty)
                    return _form[name];
                else
                    return _query[name];
            }
        }

        #endregion

        internal void SetQueryString(HttpInput query)
        {
            _query = query;
        }
        internal void SetForm(HttpInput form)
        {
            _form = form;
        }

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
            throw new NotImplementedException("Can't enumerate this class since it's a container for two others.");
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
            throw new NotImplementedException("Can't enumerate this class since it's a container for two others.");
        }

        #endregion
    }
}
