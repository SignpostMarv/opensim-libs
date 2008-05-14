//  SvnClientTest, NUnit tests for SubversionSharp
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using NUnit.Framework;

using PumaCode.SvnDotNet.SubversionSharp;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.UnitTests.SubversionSharp {
    [TestFixture]
    [Category("RepoReadOnlyTests")]
    public class SvnClientTest2 : SvnTestBase {
        private SvnClient _client;

        private SvnInfo _testInfo;
        private TestLogData[] _logInfos;
        private StringBuilder _blameData;

        #region Setup and Teardown Methods
        [TestFixtureSetUp]
        public void Init()
        {
            GetTestRepo(true);
        }

        [SetUp]
        public void SetupClient()
        {
            _client = new SvnClient();
            _testInfo = null;
            _logInfos = null;
        }

        [TearDown]
        public void TearDownClient()
        {
            _client.Pool.Destroy();
            _client = null;
        }
        #endregion

        #region The Tests
        [Test]
        public void TestBlame2()
        {
            SvnRevision rev1 = new SvnRevision(1);
            SvnRevision rev2 = SvnTestRevisions.Head;

            _blameData = new StringBuilder();

            _client.Blame2(TestRepoAbsoluteUri + "/0/04", Svn.Revision.Unspecified, rev1, rev2,
                TestBlameReceiver, IntPtr.Zero);

            string expected = GetTestResourceString("RepoBlameTest1.dat");
            string actual = _blameData.ToString();
            _blameData = null;

            Assert.AreEqual(expected, actual, "TestBlame2.01");
        }

        [Test]
        public void TestCat2()
        {
            AprStream aprStream = new AprStream(_client.Pool);
            AprFile aprFile = aprStream.AprFileInput;

            SvnStream svnStream = SvnStream.Create(aprFile, _client.Pool);
            _client.Cat2(svnStream, TestRepoAbsoluteUri + "/1/10", SvnTestRevisions.Head,
                SvnTestRevisions.Head);

            aprFile.Close();

            StreamReader actualReader = new StreamReader(aprStream);
            StreamReader expectedReader = new StreamReader(
                GetTestResourceStream("RepoCatTest1.dat"));

            CompareStreams(expectedReader, actualReader, false, "Cat2");

        }

        [Test]
        public void TestCheckout2()
        {
            string url = TestRepoAbsoluteUri + "/2/";
            string path = PathCombine(TestTempDirectory, "SvnClientTest2.TestCheckout2");

            if(Directory.Exists(path)) {
                Assert.Fail("TestCheckout2.01: '{0}' already exists", path);
            }
            Directory.CreateDirectory(path);

            _client.Checkout2(url, path, SvnTestRevisions.Unspecified,
                SvnTestRevisions.Head, false, false);

            StreamReader actualReader = new StreamReader(PathCombine(path, "20"));
            StreamReader expectedReader = new StreamReader(
                GetTestResourceStream("RepoCheckoutTest1.dat"));

            CompareStreams(expectedReader, actualReader, false, "Checkout2");

        }

        [Test]
        public void TestDiff3()
        {
            SvnRevision rev1 = SvnTestRevisions.Prev;
            SvnRevision rev2 = SvnTestRevisions.Working;

            string filePath = TestWcPath("0/04");

            AprStream stream = new AprStream(_client.Pool);
            AprFile outFile = stream.AprFileInput;
            AprFile errFile = GetTempAprFile(_client.Pool);

            _client.Diff3(SvnTestDiffOptions.None, filePath, rev1, filePath, rev2,
                false, false, true, false, "UTF-8", outFile, errFile);

            outFile.Close();
            errFile.Close();

            StreamReader actualReader = new StreamReader(stream);
            StreamReader expectedReader = new StreamReader(
                GetTestResourceStream("RepoDiffTest1.diff"));

            CompareStreams(expectedReader, actualReader, true, "Diff3");
        }

        [Test]
        public void TestDiffPeg3_1()
        {
            SvnRevision revPeg = SvnTestRevisions.Head;
            SvnRevision rev1 = SvnTestRevisions.Prev;
            SvnRevision rev2 = SvnTestRevisions.Working;

            string filePath = TestWcPath("0/04");

            AprStream stream = new AprStream(_client.Pool);
            AprFile outFile = stream.AprFileInput;
            AprFile errFile = GetTempAprFile(_client.Pool);

            _client.DiffPeg3(SvnTestDiffOptions.None, filePath, revPeg, rev1, rev2,
                false, false, true, false, "UTF-8", outFile, errFile);

            outFile.Close();
            errFile.Close();

            StreamReader actualReader = new StreamReader(stream);
            StreamReader expectedReader = new StreamReader(
                GetTestResourceStream("RepoDiffTest1.diff"));

            CompareStreams(expectedReader, actualReader, true, "DiffPeg3_1");
        }

        [Test]
        public void TestDiffPeg3_2()
        {
            SvnRevision rev1 = new SvnRevision(6);
            SvnRevision rev2 = new SvnRevision(8);
            SvnRevision revPeg = rev2;

            string filePath = TestWcPath("0/04");

            AprStream stream = new AprStream(_client.Pool);
            AprFile outFile = stream.AprFileInput;
            AprFile errFile = GetTempAprFile(_client.Pool);

            _client.DiffPeg3(SvnTestDiffOptions.None, filePath, revPeg, rev1, rev2,
                false, false, true, false, "UTF-8", outFile, errFile);

            outFile.Close();
            errFile.Close();

            StreamReader actualReader = new StreamReader(stream);
            StreamReader expectedReader = new StreamReader(
                GetTestResourceStream("RepoDiffTest2.diff"));

            CompareStreams(expectedReader, actualReader, true, "DiffPeg3_2");
        }

        [Test]
        public void TestExport3()
        {
            string url = TestRepoAbsoluteUri + "/2/20";
            string path = PathCombine(TestTempDirectory, "SvnClientTest2.TestExport3");

            if(Directory.Exists(path)) {
                Assert.Fail("TestExport3.01: '{0}' already exists", path);
            }
            Directory.CreateDirectory(path);

            path = PathCombine(path, "20");

            _client.Export3(url, path, SvnTestRevisions.Unspecified,
                SvnTestRevisions.Head, false, false, false);

            StreamReader actualReader = new StreamReader(path);
            StreamReader expectedReader = new StreamReader(
                GetTestResourceStream("RepoCheckoutTest1.dat"));

            CompareStreams(expectedReader, actualReader, false, "Export3");

        }

        [Test]
        public void TestInfo_1()
        {
            // Yes, this is almost identical to the version in SvnClientTest1, but we have
            // now relocated the repo.
            SvnRevision rev = SvnTestRevisions.Unspecified;

            _client.Info(TestWcDir, rev, rev, TestInfoReceiver, IntPtr.Zero, false);

            Assert.IsNull(_testInfo.Checksum.Value, "Info_1.01");
            Assert.IsNull(_testInfo.ConflictNew.Value, "Info_1.02");
            Assert.IsNull(_testInfo.ConflictOld.Value, "Info_1.03");
            Assert.IsNull(_testInfo.ConflictWrk.Value, "Info_1.04");
            Assert.AreEqual(-1, _testInfo.CopyFromRev, "Info_1.05");
            Assert.IsNull(_testInfo.CopyFromUrl.Value, "Info_1.06");
            Assert.IsTrue(_testInfo.HasWcInfo, "Info_1.07");
            Assert.IsFalse(_testInfo.IsNoInfo, "Info_1.08");
            Assert.IsFalse(_testInfo.IsNull, "Info_1.09");
            Assert.AreEqual(_testInfo.Kind, Svn.NodeKind.Dir, "Info_1.10");
            Assert.AreEqual(WcBaseLogEntry.Author,
                _testInfo.LastChangedAuthor.ToString(), "Info_1.11");
            Assert.AreEqual(WcBaseLogEntry.DateString,
                TimestampToDateString(_testInfo.LastChangedDate), "Info_1.12");
            Assert.AreEqual(TestWcBaseRevision, _testInfo.LastChangedRev, "Info_1.13");
            Assert.AreEqual(SvnLock.NoLock.ToString(), _testInfo.Lock.ToString(), "Info_1.14");
            Assert.IsNull(_testInfo.PrejFile.Value, "Info_1.15");
            Assert.AreEqual(0, _testInfo.PropTime, "Info_1.16");
            Assert.AreEqual(TestRepoAbsoluteUri, _testInfo.ReposRootURL.ToString(), "Info_1.17");
            Assert.AreEqual(TestWcBaseRevision, _testInfo.Rev, "Info_1.18");
            Assert.AreEqual(SvnWcEntry.WcSchedule.Normal, _testInfo.Schedule, "Info_1.19");
            Assert.AreEqual(0L, _testInfo.TextTime, "Info_1.20");
            Assert.AreEqual(TestRepoAbsoluteUri, _testInfo.URL.ToString(), "Info_1.21");
        }

        [Test]
        public void TestInfo_2()
        {
            _client.Info(TestRepoAbsoluteUri, SvnTestRevisions.Unspecified,
                SvnTestRevisions.Head, TestInfoReceiver, IntPtr.Zero, false);

            Assert.IsNull(_testInfo.Checksum.Value, "Info_2.01");
            Assert.IsNull(_testInfo.ConflictNew.Value, "Info_2.02");
            Assert.IsNull(_testInfo.ConflictOld.Value, "Info_2.03");
            Assert.IsNull(_testInfo.ConflictWrk.Value, "Info_2.04");
            Assert.AreEqual(0, _testInfo.CopyFromRev, "Info_2.05");
            Assert.IsNull(_testInfo.CopyFromUrl.Value, "Info_2.06");
            Assert.IsFalse(_testInfo.HasWcInfo, "Info_2.07");
            Assert.IsFalse(_testInfo.IsNoInfo, "Info_2.08");
            Assert.IsFalse(_testInfo.IsNull, "Info_2.09");
            Assert.AreEqual(_testInfo.Kind, Svn.NodeKind.Dir, "Info_2.10");
            Assert.AreEqual(SvnTestLog.HeadLogEntry.Author,
                _testInfo.LastChangedAuthor.ToString(), "Info_2.11");
            Assert.AreEqual(SvnTestLog.HeadLogEntry.DateString,
                TimestampToDateString(_testInfo.LastChangedDate), "Info_2.12");
            Assert.AreEqual(SvnTestLog.HeadLogEntry.Revision, _testInfo.LastChangedRev, "Info_2.13");
            Assert.AreEqual(SvnLock.NoLock.ToString(), _testInfo.Lock.ToString(), "Info_2.14");
            Assert.IsNull(_testInfo.PrejFile.Value, "Info_2.15");
            Assert.AreEqual(0, _testInfo.PropTime, "Info_2.16");
            Assert.AreEqual(TestRepoAbsoluteUri, _testInfo.ReposRootURL.ToString(), "Info_2.17");
            Assert.AreEqual(SvnTestLog.HeadLogEntry.Revision, _testInfo.Rev, "Info_2.18");
            Assert.AreEqual(SvnWcEntry.WcSchedule.Normal, _testInfo.Schedule, "Info_2.19");
            Assert.AreEqual(0L, _testInfo.TextTime, "Info_2.20");
            Assert.AreEqual(TestRepoAbsoluteUri, _testInfo.URL.ToString(), "Info_2.20");
        }

        [Test]
        public void TestList3()
        {
            string url = TestRepoAbsoluteUri + "/0";

            AprHash locks;

            AprHash aprHash = _client.List3(out locks, url, SvnTestRevisions.Unspecified,
                SvnTestRevisions.Head, false);

            TestDataStore.ClearData();

            AprString key;
            SvnDirEnt dirEnt;

            foreach(AprHashEntry hashEntry in aprHash) {
                key = hashEntry.Key;
                dirEnt = hashEntry.Value;

                TestDataStore.AddFormat("{0}|{1}|{2}|{3}|{4}\n", key.ToString(), dirEnt.Size,
                    dirEnt.LastAuthor, dirEnt.CreationRevision, dirEnt.Time);
            }

            string actual = TestDataStore.GetSortedData();
            string expected = GetTestResourceString("RepoListTest1.dat");

            Assert.AreEqual(expected, actual, "TestList3.01");
            Assert.AreEqual(locks.Count, 1, "TestList3.02");

            SvnLock lockInfo = locks.Get("05");
            Assert.IsNotNull(lockInfo, "TestList3.03");
            Assert.IsFalse(lockInfo.IsNull, "TestList3.04");
            Assert.AreEqual(1164215358987904L, lockInfo.CreationDate, "TestList3.05");
            Assert.AreEqual(0, lockInfo.ExpirationDate, "TestList3.06");
            Assert.AreEqual("sally", lockInfo.Owner.ToString(), "TestList3.06");
            Assert.AreEqual("opaquelocktoken:d4446310-cb41-4027-8444-2318d492aa8f",
                lockInfo.Token.ToString(), "TestList3.07");
        }

        [Test]
        public void TestLog2()
        {
            string[] targets = { TestRepoAbsoluteUri };
            _logInfos = new TestLogData[SvnTestLog.HeadLogEntry.Revision + 1];

            _client.Log2(targets, SvnTestRevisions.Zero, SvnTestRevisions.Head,
                SvnTestLog.HeadLogEntry.Revision + 1, false, false, TestLogReceiver, IntPtr.Zero);
            TestLogData logInfo;

            logInfo = _logInfos[0];
            Assert.AreEqual(null, logInfo.Author, "Log2.01");
            Assert.AreEqual("2006-08-22T21:40:14.084933Z", logInfo.DateString, "Log2.02");
            Assert.AreEqual(null, logInfo.Message, "Log2.03");

            logInfo = _logInfos[1];
            Assert.AreEqual("harry", logInfo.Author, "Log2.03");
            Assert.AreEqual("2006-08-22T21:41:27.653107Z", logInfo.DateString, "Log2.04");
            Assert.AreEqual("added files", logInfo.Message, "Log2.05");

            logInfo = _logInfos[2];
            Assert.AreEqual("sally", logInfo.Author, "Log2.06");
            Assert.AreEqual("2006-08-22T21:42:04.904622Z", logInfo.DateString, "Log2.07");
            Assert.AreEqual("added more files", logInfo.Message, "Log2.08");

            logInfo = _logInfos[3];
            Assert.AreEqual("harry", logInfo.Author, "Log2.09");
            Assert.AreEqual("2006-08-22T22:07:41.787782Z", logInfo.DateString, "Log2.10");
            Assert.AreEqual("modified files", logInfo.Message, "Log2.11");

            logInfo = _logInfos[4];
            Assert.AreEqual("jim", logInfo.Author, "Log2.12");
            Assert.AreEqual("2006-08-29T16:04:27.032538Z", logInfo.DateString, "Log2.13");
            Assert.AreEqual("bugfix for 0d", logInfo.Message, "Log2.14");
        }

        [Test]
        public void TestPropGet2()
        {
            string propName = "MyProp";
            string filePath = TestRepoAbsoluteUri + "/0/07";

            SvnRevision revPeg = SvnTestRevisions.Unspecified;
            SvnRevision rev1 = SvnTestRevisions.Head;

            AprHash aprHash = _client.PropGet2(propName, filePath, revPeg, rev1, false);
            Assert.IsNotNull(aprHash, "PropGet2.01");

            string expected = "another value";

            string actual = new SvnString(aprHash.Get(filePath)).ToString();

            Assert.AreEqual(expected, actual, "PropGet2.02");

        }

        //[Ignore("Uses new svn_client_proplist3 in svn 1.5")]
        //[Test]
        public void TestPropList()
        {
        }

        [Test]
        public void TestRevPropGet()
        {
            string propName = "svn:author";
            string fileUri = TestRepoAbsoluteUri + "/1/11";

            SvnRevision rev1 = SvnTestRevisions.Head;

            int revQueried;

            SvnString propValue = _client.RevPropGet(propName, fileUri, rev1, out revQueried);
            Assert.IsNotNull(propValue, "RevPropGet.01");

            Assert.AreEqual(SvnTestLog.HeadLogEntry.Author, propValue.ToString(), "PropGet2.02");
            Assert.AreEqual(SvnTestLog.HeadLogEntry.Revision, revQueried, "PropGet2.03");
        }

        [Test]
        public void TestRevPropList()
        {
            int testRevision = 3;

            SvnRevision rev1 = new SvnRevision(testRevision);
            int setRev;

            AprHash hash = _client.RevPropList(TestRepoAbsoluteUri, rev1, out setRev);
            string key;
            SvnString svnString;
            string actual;
            string expected;

            int revPropCount = 0;

            LogEntry logEntry = SvnTestLog.GetLogEntry(testRevision);

            foreach(AprHashEntry entry in hash) {
                key = entry.KeyAsString;

                switch(key) {
                    case "svn:log":
                        expected = logEntry.Message;
                        break;
                    case "svn:author":
                        expected = logEntry.Author;
                        break;
                    case "svn:date":
                        expected = logEntry.DateString;
                        break;
                    default:
                        Assert.Fail("Unexpected revprop '{0}'", key);
                        return;
                }

                svnString = new SvnString(entry.Value);
                actual = svnString.Data.ToString();

                Assert.AreEqual(expected, actual, "Comparing revprop '{0}'", key);

                revPropCount++;
            }

            Assert.AreEqual(3, revPropCount, "RevPropList.01");
        }

        [Test]
        public void TestStatus2()
        {
            string wcPath = TestWcPath("0/0d");
            TestDataStore.ClearData();

            int compareRevision = _client.Status2(wcPath, SvnTestRevisions.Unspecified,
                TestStatusReceiver, IntPtr.Zero, true, false, true, false, true);

            string actual = TestDataStore.GetSortedData();
            string expected = "0d|0d|Normal|None|False|False|jim\n";

            Assert.AreEqual(expected, actual, "TestStatus2.01");
            Assert.AreEqual(SvnTestLog.HeadRevision, compareRevision);
        }

        #endregion

        #region Delegates

        private SvnError TestInfoReceiver(IntPtr baton, SvnPath path, SvnInfo info, AprPool pool)
        {
            _testInfo = info;
            return SvnError.NoError;
        }

        private SvnError TestLogReceiver(IntPtr baton, AprHash changedPaths, int revision, AprString author,
			AprString date, AprString message, AprPool pool)
        {
            TestLogData logData = new TestLogData();
            logData.Author = author.Value;
            logData.DateString = date.Value;
            logData.Message = message.Value;

            _logInfos[revision] = logData;
            return SvnError.NoError;
        }

        private SvnError TestBlameReceiver(IntPtr baton, long lineNumber, int revision,
            AprString author, AprString date, AprString line, AprPool pool)
        {
            _blameData.AppendFormat("{0}|{1}|{2}|{3}|{4}\n", lineNumber, revision,
                author, date, line.ToString().Trim());

            return SvnError.NoError;
        }

        private void TestStatusReceiver(IntPtr baton, SvnPath path, SvnWcStatus2 status)
        {
            TestDataStore.AddFormat("{0}|{1}|{2}|{3}|{4}|{5}|{6}\n", Path.GetFileName(path.ToString()),
                status.Entry.IsNull ? "*" : status.Entry.Name.Value,
                status.TextStatus.ToString(), status.PropStatus.ToString(), status.Copied,
                status.Locked, (status.Entry.IsNull || status.Entry.CommitAuthor.IsNull) ?
                "*" : status.Entry.CommitAuthor.ToString());
        }

#endregion
    }
}

