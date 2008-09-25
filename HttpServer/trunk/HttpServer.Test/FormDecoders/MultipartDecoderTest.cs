using System;
using System.IO;
using System.Text;
using HttpServer.FormDecoders;
using NUnit.Framework;

namespace HttpServer.Test.FormDecoders
{
	[TestFixture]
	public class MultipartDecoderTest
	{
		private readonly MultipartDecoder _decoder = new MultipartDecoder();

		private readonly string requestText =
			@"-----------------------------18506757825014
Content-Disposition: form-data; name=""VCardFile""; filename=""vCard.vcf""
Content-Type: text/x-vcard

BEGIN:VCARD
N:Dikman;David;;;
EMAIL;INTERNET:a05davdi@student.his.se
ORG:Gauffin Telecom
TEL;VOICE;WORK:023-6661214
END:VCARD

-----------------------------18506757825014
Content-Disposition: form-data; name=""HiddenField[monkeyAss]""

Hejsan
-----------------------------18506757825014
Content-Disposition: form-data; name=""HiddenField[buttomAss]""

Tjosan
-----------------------------18506757825014--
";
		/// <summary> Test the parsing information function </summary>
		public void TestCanParse()
		{
			Assert.AreEqual(false, _decoder.CanParse("not-a-content-type"));
			Assert.AreEqual(true, _decoder.CanParse("multipart/form-data"));
		}

		/// <summary> Test against a null stream </summary>
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNull()
		{
			_decoder.Decode(null, "multipart/fodsa-data; boundary=---------------------------18506757825014", Encoding.UTF8);
		}

		/// <summary> Test an incorrect request content type </summary>
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestIncorrectContentType()
		{
			MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(requestText));
			_decoder.Decode(stream, "mudfsltipart/fodsa-data; boundary=---------------------------18506757825014", Encoding.UTF8);
			stream.Dispose();
		}

		/// <summary> Test an incorrect request content type </summary>
		[Test]
		[ExpectedException(typeof(InvalidDataException))]
		public void TestIncorrectContentType2()
		{
			MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(requestText));
			_decoder.Decode(stream, "multipart/form-data; boundary!---------------------------18506757825014", Encoding.UTF8);
			stream.Dispose();
		}

		/// <summary> Test an incorrect request string </summary>
		[Test]
		[ExpectedException(typeof(InvalidDataException))]
		public void TestIncorrectData()
		{
			string newRequest = requestText.Replace("Content", "snedSådan");
			MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(newRequest));
			_decoder.Decode(stream, "multipart/form-data; boundary=---------------------------18506757825014", Encoding.UTF8);
			stream.Dispose();
		}

		/// <summary> Test a correct decoding </summary>
		public void TestDecode()
		{
			MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(requestText));
			HttpForm form = _decoder.Decode(stream, "multipart/form-data; boundary=---------------------------18506757825014", Encoding.UTF8);

			Assert.IsTrue(form["HiddenField"].Contains("buttomAss"));
			Assert.IsTrue(form["HiddenField"].Contains("monkeyAss"));
			Assert.AreEqual("Hejsan", form["HiddenField"]["monkeyAss"].Value);
			Assert.AreEqual("Tjosan", form["HiddenField"]["buttomAss"].Value);
			Assert.IsNotNull(form.GetFile("VCardFile"));
			Assert.IsNotEmpty(form.GetFile("VCardFile").Filename);
			Assert.IsTrue(File.Exists(form.GetFile("VCardFile").Filename));
			form.GetFile("VCardFile").Dispose();

			stream.Dispose();
		}
	}
}
