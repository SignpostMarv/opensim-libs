using NUnit.Framework;
using HttpServer;

namespace HttpServer.Test
{
    [TestFixture]
    public class HttpInputTest
    {
        private HttpInputItem _item;

        [SetUp]
        public void Setup()
        {
            _item = new HttpInputItem("base", "esab");
        }


        [TearDown]
        public void Teardown()
        {

        }

        [Test]
        public void Test()
        {
            HttpInput input = new HttpInput("teest");
            input.Add("ivrMenu", string.Empty);
            input.Add("ivrMenu[Digits][1][Digit]", "1");
            input.Add("ivrMenu[Digits][1][Action]", "2");
            input.Add("ivrMenu[Digits][1][Value]", string.Empty);
            Assert.AreEqual("1", input["ivrMenu"]["Digits"]["1"]["Digit"].Value);
            Assert.AreEqual("2", input["ivrMenu"]["Digits"]["1"]["Action"].Value);
            Assert.AreEqual(string.Empty, input["ivrMenu"]["Digits"]["1"]["Value"].Value);
        }

        [Test]
        public void TestSimple()
        {
            HttpInput input = new HttpInput("teest");
            input.Add("ivrMenu", "myName");
            Assert.AreEqual("myName", input["ivrMenu"].Value);
        }

        [Test]
        public void TestProperties()
        {
            Assert.AreEqual("base", _item.Name, "Names do not match.");
            Assert.AreEqual("esab", _item.Value, "Value is not correct");
        }

        [Test]
        public void TestSubItems()
        {
            _item.Add("user[name]", "jonas");
            HttpInputItem item = _item["user"];
            Assert.AreEqual(null, item.Value, "Items with sub items should not have a value");

            HttpInputItem item2 = item["name"];
            Assert.AreEqual("jonas", item2.Value, "Subitems are not parsed properly");
            Assert.AreEqual("jonas", item["name"].Value, "Subitems are not parsed properly");

            Assert.AreEqual(item["name"].Name, "name");
        }

        [Test]
        public void TestMultipleValues()
        {
            _item.Add("names", "Jonas");
            _item.Add("names", "Stefan");
            _item.Add("names", "Adam");
            Assert.AreEqual(3, _item["names"].Count);
            Assert.AreEqual("Jonas", _item["names"].Values[0]);
            Assert.AreEqual("Stefan", _item["names"].Values[1]);
            Assert.AreEqual("Adam", _item["names"].Values[2]);
        }

        [Test]
        public void TestAgain()
        {
            HttpInput parent = new HttpInput("test");
            parent.Add("interception[code]", "2");
            Assert.AreEqual("2", parent["interception"]["code"].Value);
            parent.Add("theid", "2");
            Assert.AreEqual("2", parent["theid"].Value);
            parent.Add("interception[totime]", "1200");
            Assert.AreEqual("1200", parent["interception"]["totime"].Value);
        }

        [Test]
        public void TestMultipleLevels()
        {
            HttpInput parent = new HttpInput("test");
            parent.Add("user[extension][id]", "1");
            Assert.AreEqual("1", parent["user"]["extension"]["id"].Value);
        }
    }
}
