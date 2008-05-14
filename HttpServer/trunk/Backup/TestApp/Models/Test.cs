using System;

namespace TestApp.Models
{
    /// <summary>
    /// A test contains a lot of test cases that should be tested.
    /// </summary>
    public class Test
    {
        private int _id;
        private string _name;
        private int _userId;
        private DateTime _created;

        /// <summary>
        /// Owner of this test
        /// </summary>
        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        /// <summary>
        /// Name of the test
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Db id
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }
    }
}
