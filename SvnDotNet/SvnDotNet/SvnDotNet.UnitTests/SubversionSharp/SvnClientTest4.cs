//  SvnClientTest, NUnit tests for SubversionSharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

using PumaCode.SvnDotNet.SubversionSharp;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.UnitTests.SubversionSharp {
    [TestFixture]
    [Category("RepoModifyTests")]
    public class SvnClientTest4 : SvnTestBase {
        private SvnClient _client;
        private string _logMessage;
        private string _commitMessage;
        private Dictionary<string, SvnLogChangedPath> _changedPaths;
        private int _currentRevision;

        #region Setup and Teardown Methods
        [TestFixtureSetUp]
        public void Init()
        {
            GetTestRepo(true);
            _currentRevision = SvnTestLog.HeadRevision;
        }

        [SetUp]
        public void SetupClient()
        {
            _client = new SvnClient();
            _client.Context.LogMsgFunc2 = new SvnDelegate(
                (SvnClient.GetCommitLog2) GetTestCommitMessage);
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
        public void TestCommit3()
        {
            _commitMessage = "TestCommit3";
            string path = "/4/40";

            string[] paths = new string[1];
            paths[0] = TestWcPath(path);

            SvnWcStatus2 status;
            
            status = GetSingleWcStatus(paths[0]);
            Assert.AreEqual(SvnWcStatus.Kind.Modified, status.TextStatus, "TestCommit3.01");

            _client.Commit3(paths, false, false);

            CheckRepoRevisionWasBumped();

            status = GetSingleWcStatus(paths[0]);
            Assert.AreEqual(SvnWcStatus.Kind.Normal, status.TextStatus, "TestCommit3.02");

            CheckExpectedLatestLogMessage(paths[0]);
            Assert.AreEqual('M', _changedPaths[path].Action, "TestCommit3.03");
        }

        [Test]
        public void TestCopy2()
        {
            string source = "/4/41";
            string sourceUri = TestRepoAbsoluteUri + source;
            string target = "/4/41x";
            string targetUri = TestRepoAbsoluteUri + target;

            _commitMessage = "TestCopy2";

            SvnRevision revision = new SvnRevision(16);
            _client.Copy2(sourceUri, revision, targetUri);

            CheckRepoRevisionWasBumped();
            CheckExpectedLatestLogMessage(targetUri);

            SvnLogChangedPath change = _changedPaths[target];

            Assert.AreEqual('A', change.Action, "TestCopy2.01");
            Assert.AreEqual(16, change.CopyFromRev, "TestCopy2.02");
            Assert.AreEqual(source, change.CopyFromPath.ToString(), "TestCopy2.03");
        }

        [Test]
        public void TestDelete2()
        {
            string[] paths = new string[1];
            string path = "/4/42";
            paths[0] = TestRepoAbsoluteUri + path;

            string checkPath = TestRepoAbsoluteUri + "/4";

            _commitMessage = "TestDelete2";

            _client.Delete2(paths, false);

            CheckRepoRevisionWasBumped();
            CheckExpectedLatestLogMessage(checkPath);

            SvnLogChangedPath change = _changedPaths[path];
            Assert.AreEqual('D', change.Action, "TestDelete2.01");
        }

        [Test]
        public void TestImport2()
        {
            string source = PathCombine(TestBaseDataDirectory, "imp");
            string target = "/4/4z";
            string targetUri = TestRepoAbsoluteUri + target;

            string[] paths = new string[1];
            paths[0] = targetUri;

            _commitMessage = "TestImport2-Mkdir";

            _client.Mkdir2(paths);

            CheckRepoRevisionWasBumped();
            CheckExpectedLatestLogMessage(paths[0]);

            SvnLogChangedPath change = _changedPaths[target];
            Assert.AreEqual('A', change.Action, "TestImport2.01");

            _commitMessage = "TestImport2-Import";

            _client.Import2(source, targetUri, false, false);

            CheckRepoRevisionWasBumped();
            CheckExpectedLatestLogMessage(paths[0]);

            change = _changedPaths[target + "/0"];
            Assert.AreEqual('A', change.Action, "TestImport2.01");

        }

        [Test]
        public void TestLock()
        {
            string[] paths = new string[1];
            string path = "/4/47";
            paths[0] = TestRepoAbsoluteUri + path;

            string comment = "My Lock Comment";

            SetUsernameAuthProvider(_client);
            _client.Lock(paths, comment, false);

            SvnInfo info = GetSingleInfo(paths[0]);
            Assert.AreEqual(comment, info.Lock.Comment.ToString(), "TestLock.01");
            Assert.AreNotEqual(0L, info.Lock.CreationDate, "TestLock.02");
            Assert.AreEqual(TestUsername, info.Lock.Owner.ToString(), "TestLock.03");
            Assert.IsFalse(info.Lock.Token.IsNull, "TestLock.04");
        }

        [Test]
        public void TestMkdir2()
        {
            string[] paths = new string[1];
            string path = "/4/4y";
            paths[0] = TestRepoAbsoluteUri + path;

            _commitMessage = "TestMkdir2";

            _client.Mkdir2(paths);

            CheckRepoRevisionWasBumped();
            CheckExpectedLatestLogMessage(paths[0]);

            SvnLogChangedPath change = _changedPaths[path];
            Assert.AreEqual('A', change.Action, "TestMkdir2.01");
        }

        [Test]
        public void TestMove3()
        {
            string source = "/4/45";
            string sourceUri = TestRepoAbsoluteUri + source;
            string target = "/4/45x";
            string targetUri = TestRepoAbsoluteUri + target;

            _commitMessage = "TestMove3";
            int currentRevision = _currentRevision;

            _client.Move3(sourceUri, targetUri, false);

            CheckRepoRevisionWasBumped();
            CheckExpectedLatestLogMessage(targetUri);

            SvnLogChangedPath change = _changedPaths[source];
            Assert.AreEqual('D', change.Action, "TestMove3.01");

            change = _changedPaths[target];
            Assert.AreEqual('A', change.Action, "TestMove3.02");
            Assert.AreEqual(currentRevision, change.CopyFromRev, "TestMove3.03");
            Assert.AreEqual(source, change.CopyFromPath.ToString(), "TestMove3.04");
        }

        [Test]
        public void TestRevPropSet()
        {
            string target = "/4/46";
            string targetUri = TestRepoAbsoluteUri + target;

            string name = "TestPropSet2";
            string value = "MyValue";
            SvnString valueSvnString = new SvnString(value, _client.Pool);

            _commitMessage = "TestRevPropSet";
            int currentRevision = _currentRevision;
            SvnRevision revision = new SvnRevision(currentRevision);

            _client.RevPropSet(name, valueSvnString, targetUri, revision,
                false);

            int revQueried;
            SvnString propValue = _client.RevPropGet(name, targetUri, revision, out revQueried);
            Assert.IsNotNull(propValue, "RevPropSet.01");

            Assert.AreEqual(value, propValue.ToString(), "PropGet2.02");
            Assert.AreEqual(currentRevision, revQueried, "PropGet2.03");
        }

        [Test]
        public void TestUnLock()
        {
            string[] paths = new string[1];
            string path = "/4/48";
            paths[0] = TestRepoAbsoluteUri + path;

            string comment = "My Lock Comment";

            SetUsernameAuthProvider(_client);
            _client.Lock(paths, comment, false);

            SvnInfo info;

            info = GetSingleInfo(paths[0]);
            Assert.AreEqual(comment, info.Lock.Comment.ToString(), "TestLock.01");
            Assert.AreNotEqual(0L, info.Lock.CreationDate, "TestLock.02");
            Assert.AreEqual(TestUsername, info.Lock.Owner.ToString(), "TestLock.03");
            Assert.IsFalse(info.Lock.Token.IsNull, "TestLock.04");

            _client.Unlock(paths, false);

            info = GetSingleInfo(paths[0]);
            Assert.IsTrue(info.Lock.IsNull, "TestLock.05");
        }

        #endregion

        private void CheckRepoRevisionWasBumped()
        {
            SvnInfo info = GetSingleInfo(TestRepoAbsoluteUri);
            _currentRevision++;

            Assert.AreEqual(_currentRevision, info.LastChangedRev, "CheckRepoRevisionWasBumped");
        }

        private void CheckExpectedLatestLogMessage(string path)
        {
            GetLatestLogMessage(path);
            Assert.AreEqual(_commitMessage, _logMessage, "CheckExpectedLatestLogMessage");
        }

        private void GetLatestLogMessage(string path)
        {
            string[] targets = new string[1];
            targets[0] = path;
            _logMessage = null;

            _client.Log2(targets, SvnTestRevisions.Head, SvnTestRevisions.Head, 1, true,
                true, TestLogReceiver, IntPtr.Zero);
        }

        private SvnError GetTestCommitMessage(out AprString logMessage, out SvnPath tmpFile,
                                              AprArray commitItems, IntPtr baton,
                                              AprPool pool)
        {
            logMessage = new AprString(_commitMessage, pool);
            tmpFile = new SvnPath(IntPtr.Zero);
            return SvnError.NoError;
        }

        private SvnError TestLogReceiver(IntPtr baton, AprHash changedPaths, int revision, AprString author,
            AprString date, AprString message, AprPool pool)
        {
            _logMessage = message.Value;

            _changedPaths = new Dictionary<string, SvnLogChangedPath>();

            foreach(AprHashEntry entry in changedPaths) {
                _changedPaths[entry.KeyAsString] = new SvnLogChangedPath(entry.Value);
            }

            return SvnError.NoError;
        }

    }
}