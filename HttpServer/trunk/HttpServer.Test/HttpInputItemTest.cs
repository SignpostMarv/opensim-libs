using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace HttpServer.Test
{
	[TestFixture]
	public class HttpInputItemTest
	{
		public void Test()
		{
			HttpInputItem item = new HttpInputItem("Name", "Value");
			Assert.AreEqual("Value", item.Value);

			item.Add("value2");
			Assert.AreEqual(item.Values.Count, 2);
			Assert.AreEqual("value2", item.Values[1]);

			item.Add("subName", "subValue");
			Assert.AreEqual("subValue", item["subName"].Value);
		}
	}
}
