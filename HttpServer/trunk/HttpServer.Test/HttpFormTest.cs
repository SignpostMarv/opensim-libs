using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace HttpServer.Test
{
	[TestFixture]
	public class HttpFormTest
	{
		HttpForm _form = new HttpForm();

		#region EmptyForm tests
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestEmptyAddFile()
		{
			HttpForm.EmptyForm.AddFile(null);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestEmptyGetFile()
		{
			HttpForm.EmptyForm.GetFile(null);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestEmptyContainsFile()
		{
			HttpForm.EmptyForm.ContainsFile(null);
		}
		#endregion


		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestArgumentNullAddFile()
		{
			_form.AddFile(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestArgumentNullGetFile()
		{
			_form.GetFile(string.Empty);
		}

		public void TestModifications()
		{
			HttpFile file = new HttpFile("testFile", "nun", "nun");
			_form.AddFile(file);
			Assert.AreEqual(file, _form.GetFile("testFile"));

			_form.Add("valueName", "value");
			Assert.AreEqual("value", _form["valueName"].Value);

			_form.Clear();
			Assert.IsNull(_form.GetFile("testFile"));
			Assert.IsNull(_form["valueName"].Value);
		}
	}
}
