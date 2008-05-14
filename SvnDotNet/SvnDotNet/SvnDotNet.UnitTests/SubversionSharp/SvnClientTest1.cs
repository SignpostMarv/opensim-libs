//  SvnClientTest, NUnit tests for SubversionSharp

using System;
using System.IO;
using System.Text;

using NUnit.Framework;
using PumaCode.SvnDotNet.AprSharp;
using PumaCode.SvnDotNet.SubversionSharp;

namespace PumaCode.SvnDotNet.UnitTests.SubversionSharp {
    [TestFixture]
    [Category("LocalReadOnlyTests")]
    public class SvnClientTest1 : SvnTestBase {
        private SvnClient _client;

        private SvnInfo _testInfo;

        #region Setup and Teardown Methods
        [TestFixtureSetUp]
        public void Init()
        {
            GetTestRepo(false);
        }

        [SetUp]
        public void SetupClient()
        {
            _client = new SvnClient();
            _testInfo = null;
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
        public void TestCreateDestroy()
        {
            Assert.IsNotNull(_client, "CreateDestroy.01");

            AprPool p = Svn.PoolCreate();
            _client = new SvnClient(p);
            Assert.IsNotNull(_client, "CreateDestroy.02");

            SvnClientContext ctx = SvnClientContext.Create(p);
            ctx.Config = SvnConfig.GetConfig(p);
            _client = new SvnClient(ctx, p);
            Assert.IsNotNull(_client, "CreateDestroy.03");

            p.Destroy();
            Assert.IsTrue(p.IsNull, "CreateDestroy.04");

        }

        [Test]
        public void TestDiff3()
        {
            SvnRevision rev1 = SvnTestRevisions.Base;
            SvnRevision rev2 = SvnTestRevisions.Working;

            string filePath = TestWcPath("0/02");

            AprStream stream = new AprStream(_client.Pool);
            AprFile outFile = stream.AprFileInput;
            AprFile errFile = GetTempAprFile(_client.Pool);

            _client.Diff3(SvnTestDiffOptions.None, filePath, rev1, filePath, rev2,
                false, false, true, false, "UTF-8", outFile, errFile);

            outFile.Close();
            errFile.Close();

            StreamReader actualReader = new StreamReader(stream);
            StreamReader expectedReader = new StreamReader(
                GetTestResourceStream("WcDiffTest1.diff"));

            CompareStreams(expectedReader, actualReader, true, "Diff3");
        }

        [Test]
        public void TestInfo()
        {

            SvnRevision rev = SvnTestRevisions.Unspecified;

            _client.Info(TestWcDir, rev, rev, TestInfoReceiver, IntPtr.Zero, false);

            Assert.IsNull(_testInfo.Checksum.Value, "Info.01");
            Assert.IsNull(_testInfo.ConflictNew.Value, "Info.02");
            Assert.IsNull(_testInfo.ConflictOld.Value, "Info.03");
            Assert.IsNull(_testInfo.ConflictWrk.Value, "Info.04");
            Assert.AreEqual(-1, _testInfo.CopyFromRev, "Info.05");
            Assert.IsNull(_testInfo.CopyFromUrl.Value, "Info.06");
            Assert.IsTrue(_testInfo.HasWcInfo, "Info.07");
            Assert.IsFalse(_testInfo.IsNoInfo, "Info.08");
            Assert.IsFalse(_testInfo.IsNull, "Info.09");
            Assert.AreEqual(_testInfo.Kind, Svn.NodeKind.Dir, "Info.10");
            Assert.AreEqual(WcBaseLogEntry.Author,
                _testInfo.LastChangedAuthor.ToString(), "Info.11");
            Assert.AreEqual(WcBaseLogEntry.DateString,
                TimestampToDateString(_testInfo.LastChangedDate), "Info.12");
            Assert.AreEqual(TestWcBaseRevision, _testInfo.LastChangedRev, "Info.13");
            Assert.AreEqual(SvnLock.NoLock.ToString(), _testInfo.Lock.ToString(), "Info.14");
            Assert.IsNull(_testInfo.PrejFile.Value, "Info.15");
            Assert.AreEqual(0, _testInfo.PropTime, "Info.16");
            Assert.AreEqual(TestRepoFakeRootUri, _testInfo.ReposRootURL.ToString(), "Info.17");
            Assert.AreEqual(TestWcBaseRevision, _testInfo.Rev, "Info.18");
            Assert.AreEqual(SvnWcEntry.WcSchedule.Normal, _testInfo.Schedule, "Info.19");
            Assert.AreEqual(0L, _testInfo.TextTime, "Info.20");
            Assert.AreEqual(TestRepoFakeRootUri, _testInfo.URL.ToString(), "Info.20");
        }

        [Test]
        public void TestPropGet2()
        {
            string propName = "MyProp";
            string filePath = TestWcPath("0/06");

            SvnRevision revPeg = SvnTestRevisions.Unspecified;
            SvnRevision rev1 = SvnTestRevisions.Working;

            AprHash aprHash = _client.PropGet2(propName, filePath, revPeg, rev1, false);
            Assert.IsNotNull(aprHash, "PropGet2.01");

            string expected = "my property value";
            
            string actual = new SvnString(aprHash.Get(filePath.Replace('\\', '/'))).ToString();

            Assert.AreEqual(expected, actual, "PropGet2.02");
        }

        [Test]
        public void TestPropList()
        {
            string filePath = TestWcPath("0/06");

            SvnRevision rev1 = SvnTestRevisions.Working;

            AprArray aprArray = _client.PropList(filePath, rev1, false);

            Assert.IsNotNull(aprArray, "PropList.01");
            Assert.AreEqual(1, aprArray.Count, "PropList.02");
            aprArray.ElementType = typeof(SvnClientPropListItem);

            SvnClientPropListItem[] aprItems = new SvnClientPropListItem[aprArray.Count];
            aprArray.CopyTo(aprItems, 0);

            string expected = "MyProp";
            string actual = "";
            foreach (AprHashEntry hashEntry in aprItems[0].PropHash) {
                actual += hashEntry.KeyAsString;
            }

            Assert.AreEqual(expected, actual, "PropList.03");

        }

        [Test]
        public void TestStatus2()
        {
            string wcPath = TestWcPath("0");
            TestDataStore.ClearData();

            _client.Status2(wcPath, SvnTestRevisions.Working, TestStatusReceiver,
                IntPtr.Zero, true, true, false, false, true);

            string actual = TestDataStore.GetSortedData();
            string expected = GetTestResourceString("WcStatusTest1.dat");

            Assert.AreEqual(expected, actual, "TestStatus2.01");
        }

        #endregion

        #region Delegates
        private SvnError TestInfoReceiver(IntPtr baton, SvnPath path, SvnInfo info, AprPool pool)
        {
            _testInfo = info;
            return SvnError.NoError;
        }

        private void TestStatusReceiver(IntPtr baton, SvnPath path, SvnWcStatus2 status)
        {
            TestDataStore.AddFormat("{0}|{1}|{2}|{3}|{4}|{5}|{6}\n", Path.GetFileName(path.ToString()),
                status.Entry.IsNull? "*" : status.Entry.Name.Value,
                status.TextStatus.ToString(), status.PropStatus.ToString(), status.Copied,
                status.Locked, (status.Entry.IsNull || status.Entry.CommitAuthor.IsNull)?
                "*" : status.Entry.CommitAuthor.ToString());
        }
        #endregion
    }
}

