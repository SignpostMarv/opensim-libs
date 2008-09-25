using System;
using System.Net;
using System.Text.RegularExpressions;
using HttpServer.Rules;
using HttpServer.Test.TestHelpers;
using NUnit.Framework;

namespace HttpServer.Test
{
	[TestFixture]
	public class RegexRedirectRuleTest
	{
		#region Null tests
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNullParameter0()
		{
			new RegexRedirectRule(null, "nun");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNullParameter1()
		{
			new RegexRedirectRule("", "nun");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNullParameter2()
		{
			new RegexRedirectRule("nun", "");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNullParameter3()
		{
			RegexRedirectRule regexRule = new RegexRedirectRule("nun", "nun");
			regexRule.Process(null, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNullParameter4()
		{
			RegexRedirectRule regexRule = new RegexRedirectRule("nun", "nun");
			regexRule.Process(new HttpTestRequest(), null);
		}
		#endregion

		public void TestCorrect()
		{
			RegexRedirectRule rule = new RegexRedirectRule("/(?<first>[a-z]+)/(?<second>[a-z]+/?)", "/test/?parameter=${second}&ignore=${first}", RegexOptions.IgnoreCase);
            IHttpRequest request = new HttpTestRequest();
			request.HttpVersion = "1.0";
			request.Uri = new Uri("http://www.google.com/above/all/", UriKind.Absolute);
			IHttpResponse response = new HttpResponse(null, request);
			rule.Process(request, response);
			Assert.AreEqual(HttpStatusCode.Redirect, response.Status);
		}

        public void TestCorrectNoRedirect()
        {
            RegexRedirectRule rule = new RegexRedirectRule("/(?<first>[a-z]+)/(?<second>[a-z]+)/?", "/test/?ignore=${second}&parameters=${first}", RegexOptions.IgnoreCase, false);
            IHttpRequest request = new HttpTestRequest();
            request.HttpVersion = "1.0";
            request.Uri = new Uri("http://www.google.com/above/all/", UriKind.Absolute);
            IHttpResponse response = new HttpResponse(null, request);
            rule.Process(request, response);
            Assert.AreEqual(request.Uri.ToString(), "http://www.google.com/test/?ignore=all&parameters=above");
        }
	}
}
