using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using HttpServer.Rendering;

namespace HttpServer.Test.Renderers
{
    /// <summary>
    /// Contains tests for the TemplateArgument class and subclass ArgumentContainer.
    /// Most functions tests the ArgumentContainer class automagically because those are the ones actually throwing the
    /// exceptions.
    /// </summary>
    [TestFixture]
    public class TemplateArgumentsTest
    {
        private TemplateArguments _arguments = null;

        /// <summary>
        /// Test to check so that a user must pass a valid object 
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNoTypeSubmittedTo()
        {
            _arguments = new TemplateArguments("User", null);
        }

        /// <summary>
        /// Test to check so that a user must pass a valid name for an object 
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullStringSubmitted()
        {
            _arguments = new TemplateArguments(null, 1);
        }

        /// <summary>
        /// Test to make sure types must correspond
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestWrongTypeSubmitted()
        {
            _arguments = new TemplateArguments();
            _arguments.Add("TestString", 4, typeof(float));
        }

        /// <summary>
        /// Test to make sure duplicates are noticed
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDuplicate()
        {
            _arguments = new TemplateArguments("Test", 1);
            _arguments.Add("Test", 2);
        }

        /// <summary>
        /// Test to make sure null objects cannot be passed without type information
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullObject()
        {
            _arguments = new TemplateArguments();
            _arguments.Add("Test", null);
        }

        /// <summary>
        /// Tests to make sure no nonexisting value can be updated
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNonExisting()
        {
            _arguments = new TemplateArguments();
            _arguments.Update("Test", 2);
        }
    }
}