using System.Collections.Specialized;
using Fadd;
using Fadd.Globalization;
using NUnit.Framework;

namespace HttpServer.Test.Helpers
{
	[TestFixture]
	public class FormValidatorTest
	{
		NameValueCollection _errors;
		Validator _validator;

		[SetUp]
		public void SetUp()
		{
			_errors = new NameValueCollection();
			_validator = new Validator(_errors, LanguageNode.Empty);
		}

		public void TestHex()
		{
			_errors.Clear();
			Assert.AreEqual("1234567890ABCDEF", _validator.Hex("hex", "1234567890ABCDEF", false));
			Assert.AreEqual("abcdef1234567890", _validator.Hex("hex", "abcdef1234567890", false));
			Assert.AreEqual(string.Empty, _validator.Letters(null, null, false));
			Assert.AreEqual(0, _errors.Count);

			Assert.AreEqual(string.Empty, _validator.Hex("hex", "###", false));
			Assert.AreEqual(1, _errors.Count);
			_errors.Clear();

			Assert.AreEqual(string.Empty, _validator.Hex("hex", "", true));
			Assert.AreEqual(1, _errors.Count);
			_errors.Clear();
		}

		public void TestLetters()
		{
			_errors.Clear();
			Assert.AreEqual("abcde XYZ", _validator.Letters(string.Empty, "abcde XYZ", false));
			Assert.AreEqual("abcdeXYZ≈ƒ÷Â‰ˆ", _validator.Letters(string.Empty, "abcdeXYZ≈ƒ÷Â‰ˆ", false));
			Assert.AreEqual("abc ab ab c", _validator.Letters(string.Empty, "abc ab ab c", false, "abc abc abc"));
			Assert.AreEqual(string.Empty, _validator.Letters(null, null, false));
			Assert.AreEqual(0, _errors.Count);

			Assert.AreEqual(string.Empty, _validator.Letters(string.Empty, string.Empty, true));
			Assert.AreEqual(1, _errors.Count);
			_errors.Clear();

			Assert.AreEqual(string.Empty, _validator.Letters(string.Empty, "Test206", false));
			Assert.AreEqual(1, _errors.Count);
			_errors.Clear();

			Assert.AreEqual(string.Empty, _validator.Letters(string.Empty, "Test206", false, " 1"));
			Assert.AreEqual(1, _errors.Count);
			_errors.Clear();

			Assert.AreEqual(string.Empty, _validator.Letters(string.Empty, "Test206", false, " 1"));
			Assert.AreEqual(1, _errors.Count);
			_errors.Clear();
		}
	}
}
