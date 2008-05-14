using System;

namespace TestApp.Models
{
    /// <summary>
    /// A test case identifies a specific test that should be made.
    /// </summary>
    public class TestCase
    {
        private int _id;
        private int _testId;
        private string _name;
        private string _category;
        private string _description;
        private TestState _state;
        private DateTime _created;

        /// <summary>
        /// current state of the test.
        /// </summary>
        public TestState State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// Id of this specific case.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Test suite that this case belongs to.
        /// </summary>
        public int TestId
        {
            get { return _testId; }
            set { _testId = value; }
        }

        /// <summary>
        /// Name of the test.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Test cases can be splitted up into categories and sub categoires.
        /// Use " / " as category splitter
        /// </summary>
        /// <example>
        /// case.Category = "main / models / user"
        /// </example>
        /// <remarks>
        /// This propery will later on be used to create a new table containing all
        /// categories.</remarks>
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        /// <summary>
        /// Describes how the test should be made.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }
    }

    public enum TestState
    {
        NotTested,
        Failed,
        Successful
    }
}
