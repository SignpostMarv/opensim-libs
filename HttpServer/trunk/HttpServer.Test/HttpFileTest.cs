using System;
using System.IO;
using NUnit.Framework;

namespace HttpServer.Test
{
	[TestFixture]
	public class HttpFileTest
	{
		/// <summary> Test to make sure files gets deleted upon disposing </summary>
		public void TestFileDeletion()
		{
			string path = Environment.CurrentDirectory + "\\tmptest";
			HttpFile file = new HttpFile("testFile", path, "nun");

			File.WriteAllText(path, "test");
			file.Dispose();

			Assert.AreEqual(File.Exists(path), false);
		}

		#region Disposed tests

		/// <summary> Test to make sure name cannot be retrieved from disposed object </summary>
		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void TestDisposedName()
		{
			HttpFile file = new HttpFile("object", "object", "object");
			file.Dispose();

			#pragma warning disable 168
				string tmp = file.Name;
			#pragma warning restore 168
		}

		/// <summary> Test to make sure contenttype cannot be retrieved from disposed object </summary>
		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void TestDisposedContent()
		{
			HttpFile file = new HttpFile("object", "object", "object");
			file.Dispose();

			#pragma warning disable 168
				string tmp = file.ContentType;
			#pragma warning restore 168
		}

		/// <summary> Test to make sure filename cannot be retrieved from disposed object </summary>
		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void TestDisposedFilename()
		{
			HttpFile file = new HttpFile("object", "object", "object");
			file.Dispose();

			#pragma warning disable 168
				string tmp = file.Filename;
			#pragma warning restore 168
		}
		#endregion

		#region Null tests
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNullName()
		{
			HttpFile file = new HttpFile(null, null, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNullFile()
		{
			HttpFile file = new HttpFile("nun", null, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNullContent()
		{
			HttpFile file = new HttpFile("nun", "nun", null);
		}
		#endregion
	}
}
