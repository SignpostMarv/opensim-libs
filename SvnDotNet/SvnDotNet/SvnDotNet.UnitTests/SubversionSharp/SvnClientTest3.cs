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
    [Category("WcModifyTests")]
    public class SvnClientTest3 : SvnTestBase {
        private SvnClient _client;

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
        public void TestAdd3()
        {
            string wcPath = TestWcPath("0/0x");

            SvnWcStatus2 status;

            status = GetSingleWcStatus(wcPath);
            Assert.AreEqual(SvnWcStatus.Kind.Unversioned, status.TextStatus,
                "TestAdd3.01");

            _client.Add3(wcPath, false, false, false);

            status = GetSingleWcStatus(wcPath);
            Assert.AreEqual(SvnWcStatus.Kind.Added, status.TextStatus,
                "TestAdd3.02");
        }

        [Test]
        public void TestCleanUp()
        {
            // This was already tested when relocating the repo, so we get a free green light here :)
        }

        [Test]
        public void TestCopy2_1()
        {
            string fromPath = TestWcPath("2/20");
            string toPath = TestWcPath("2/2x");

            _client.Copy2(fromPath, SvnTestRevisions.Unspecified, toPath);

            SvnWcStatus2 status = GetSingleWcStatus(toPath);
            Assert.AreEqual(SvnWcStatus.Kind.Added, status.TextStatus, "TestCopy2_1.01");
            Assert.IsTrue(status.Copied, "TestCopy2_1.02");
        }

        [Test]
        public void TestCopy2_2()
        {
            string fromPath = TestRepoAbsoluteUri + "/2/21";
            string toPath = TestWcPath("2/2y");

            _client.Copy2(fromPath, SvnTestRevisions.Head, toPath);

            SvnWcStatus2 status = GetSingleWcStatus(toPath);
            Assert.AreEqual(SvnWcStatus.Kind.Added, status.TextStatus, "TestCopy2_2.01");
            Assert.IsTrue(status.Copied, "TestCopy2_2.02");
            Assert.AreEqual(status.Entry.CopyFromRevision, SvnTestLog.HeadRevision,
                "TestCopy2_2.03");
        }

        [Test]
        public void TestDelete2()
        {
            string[] paths = new string[1];
            paths[0] = TestWcPath("2/22");

            SvnWcStatus2 status;
            status = GetSingleWcStatus(paths[0]);
            Assert.AreEqual(SvnWcStatus.Kind.Normal, status.TextStatus, "TestDelete2.01");

            _client.Delete2(paths, false);

            status = GetSingleWcStatus(paths[0]);
            Assert.AreEqual(SvnWcStatus.Kind.Deleted, status.TextStatus, "TestDelete2.02");
        }

        [Test]
        public void TestLock()
        {
            string[] paths = new string[1];
            paths[0] = TestWcPath("2/23");
            string comment = "Test Lock";

            SvnWcStatus2 status;
            status = GetSingleWcStatus(paths[0]);
            Assert.IsTrue(status.Entry.LockComment.IsNull, "TestLock.01");
            Assert.AreEqual(0L, status.Entry.LockCreationDate, "TestLock.02");
            Assert.IsTrue(status.Entry.LockOwner.IsNull, "TestLock.03");
            Assert.IsTrue(status.Entry.LockToken.IsNull, "TestLock.04");

            SetUsernameAuthProvider(_client);
            _client.Lock(paths, comment, false);

            status = GetSingleWcStatus(paths[0]);
            Assert.AreEqual(comment, status.Entry.LockComment.ToString(), "TestLock.05");
            Assert.AreNotEqual(0L, status.Entry.LockCreationDate, "TestLock.06");
            Assert.AreEqual(TestUsername, status.Entry.LockOwner.ToString(), "TestLock.07");
            Assert.IsFalse(status.Entry.LockToken.IsNull, "TestLock.08");
        }

        [Test]
        public void TestMerge()
        {
            string source = TestRepoAbsoluteUri + "/3b";
            string target = TestWcPath("3");
            string checkPath = TestWcPath("3/30");

            SvnRevision svnRev1 = new SvnRevision(13);
            SvnRevision svnRev2 = new SvnRevision(14);

            SvnWcStatus2 status;
            status = GetSingleWcStatus(checkPath);
            Assert.AreEqual(SvnWcStatus.Kind.Normal, status.TextStatus, "TestMerge.01");

            _client.Merge(source, svnRev1, source, svnRev2, target, true, false, false, false);

            status = GetSingleWcStatus(checkPath);
            Assert.AreEqual(SvnWcStatus.Kind.Modified, status.TextStatus, "TestMerge.02");
        }

        [Test]
        public void TestMergePeg()
        {
            string source = TestRepoAbsoluteUri + "/3b";
            string target = TestWcPath("3");
            string checkPath = TestWcPath("3/31");

            SvnRevision svnRev1 = new SvnRevision(14);
            SvnRevision svnRev2 = new SvnRevision(15);

            SvnWcStatus2 status;
            status = GetSingleWcStatus(checkPath);
            Assert.AreEqual(SvnWcStatus.Kind.Normal, status.TextStatus, "TestMergePeg.01");

            _client.Merge(source, svnRev1, svnRev2, svnRev2, target, true, false, false, false);

            status = GetSingleWcStatus(checkPath);
            Assert.AreEqual(SvnWcStatus.Kind.Modified, status.TextStatus, "TestMergePeg.02");
        }

        [Test]
        public void TestMkdir2()
        {
            string[] paths = new string[1];
            paths[0] = TestWcPath("xx");

            SvnWcStatus2 status;
            status = GetSingleWcStatus(paths[0]);
            Assert.IsNull(status, "TestMkdir2.01");

            _client.Mkdir2(paths);

            status = GetSingleWcStatus(paths[0]);
            Assert.AreEqual(SvnWcStatus.Kind.Added, status.TextStatus, "TestMkdir2.02");
        }

        [Test]
        public void TestMove3()
        {
            string source = TestWcPath("3/3a");
            string destination = TestWcPath("3/3x");

            SvnWcStatus2 status;
            status = GetSingleWcStatus(source);
            Assert.AreEqual(SvnWcStatus.Kind.Normal, status.TextStatus, "TestMove3.01");

            _client.Move3(source, destination, false);

            status = GetSingleWcStatus(source);
            Assert.AreEqual(SvnWcStatus.Kind.Deleted, status.TextStatus, "TestMove3.02");

            status = GetSingleWcStatus(destination);
            Assert.AreEqual(SvnWcStatus.Kind.Added, status.TextStatus, "TestMove3.03");
            Assert.IsTrue(status.Copied, "TestMove3.03");
        }

        [Test]
        public void TestPropSet2()
        {
            string target = TestWcPath("3/3b");
            string name = "TestPropSet2";

            string value = "MyValue";
            SvnString valueSvnString = new SvnString(value, _client.Pool);

            SvnWcStatus2 status;
            status = GetSingleWcStatus(target);
            Assert.AreEqual(SvnWcStatus.Kind.None, status.PropStatus, "TestPropSet2.01");

            _client.PropSet2(name, valueSvnString, target, false, false);

            status = GetSingleWcStatus(target);
            Assert.AreEqual(SvnWcStatus.Kind.Modified, status.PropStatus, "TestPropSet2.02");

            SvnRevision revPeg = SvnTestRevisions.Unspecified;
            SvnRevision rev1 = SvnTestRevisions.Working;

            AprHash aprHash = _client.PropGet2(name, target, revPeg, rev1, false);
            Assert.IsNotNull(aprHash, "PropSet2.02");

            string actual = new SvnString(aprHash.Get(target.Replace('\\', '/'))).ToString();
            Assert.AreEqual(value, actual, "PropSet2.03");
        }

        [Test]
        public void TestRelocate()
        {
            // This was already tested when relocating the repo, so we get a free green light here :)
        }

        [Test]
        public void TestResolved()
        {
            string source = TestRepoAbsoluteUri + "/3b";
            string target = TestWcPath("3");
            string checkPath = TestWcPath("3/32");

            SvnRevision svnRev1 = new SvnRevision(15);
            SvnRevision svnRev2 = new SvnRevision(16);

            SvnWcStatus2 status;
            status = GetSingleWcStatus(checkPath);
            Assert.AreEqual(SvnWcStatus.Kind.Modified, status.TextStatus, "TestResolved.01");

            _client.Merge(source, svnRev1, source, svnRev2, target, true, false, false, false);

            status = GetSingleWcStatus(checkPath);
            Assert.AreEqual(SvnWcStatus.Kind.Conflicted, status.TextStatus, "TestResolved.02");

            _client.Resolved(checkPath, false);

            status = GetSingleWcStatus(checkPath);
            Assert.AreEqual(SvnWcStatus.Kind.Modified, status.TextStatus, "TestResolved.03");

        }

        [Test]
        public void TestRevert()
        {
            string[] paths = new string[1];
            paths[0] = TestWcPath("2/25");

            SvnWcStatus2 status;
            status = GetSingleWcStatus(paths[0]);
            Assert.AreEqual(SvnWcStatus.Kind.Modified, status.TextStatus, "TestRevert.01");

            _client.Revert(paths, false);

            status = GetSingleWcStatus(paths[0]);
            Assert.AreEqual(SvnWcStatus.Kind.Normal, status.TextStatus, "TestRevert.02");
        }

        [Test]
        public void TestUnlock()
        {
            string[] paths = new string[1];
            paths[0] = TestWcPath("2/24");
            string comment = "Test Lock";

            SvnWcStatus2 status;
            status = GetSingleWcStatus(paths[0]);
            Assert.IsTrue(status.Entry.LockComment.IsNull, "TestUnlock.01");
            Assert.AreEqual(0L, status.Entry.LockCreationDate, "TestUnlock.02");
            Assert.IsTrue(status.Entry.LockOwner.IsNull, "TestUnlock.03");
            Assert.IsTrue(status.Entry.LockToken.IsNull, "TestUnlock.04");

            SetUsernameAuthProvider(_client);
            _client.Lock(paths, comment, false);

            status = GetSingleWcStatus(paths[0]);
            Assert.AreEqual(comment, status.Entry.LockComment.ToString(), "TestUnlock.05");
            Assert.AreNotEqual(0L, status.Entry.LockCreationDate, "TestUnlock.06");
            Assert.AreEqual(TestUsername, status.Entry.LockOwner.ToString(), "TestUnlock.07");
            Assert.IsFalse(status.Entry.LockToken.IsNull, "TestUnlock.08");

            _client.Unlock(paths, false);

            status = GetSingleWcStatus(paths[0]);
            Assert.IsTrue(status.Entry.LockComment.IsNull, "TestUnlock.09");
            Assert.AreEqual(0L, status.Entry.LockCreationDate, "TestUnlock.10");
            Assert.IsTrue(status.Entry.LockOwner.IsNull, "TestUnlock.11");
            Assert.IsTrue(status.Entry.LockToken.IsNull, "TestUnlock.12");
        }

        [Test]
        public void TestUpdate2()
        {
            string[] paths = new string[1];
            paths[0] = TestWcPath("0/07");
            int lastChangedRev = 12;

            SvnWcStatus2 status;
            status = GetSingleWcStatus(paths[0]);
            Assert.AreEqual(lastChangedRev, status.Entry.CommitRev, "TestUpdate2.01");
            Assert.AreEqual(lastChangedRev, status.Entry.Revision, "TestUpdate2.02");

            _client.Update2(paths, SvnTestRevisions.Head, false, true);

            status = GetSingleWcStatus(paths[0]);
            Assert.AreEqual(lastChangedRev, status.Entry.CommitRev, "TestUpdate2.03");
            Assert.AreEqual(SvnTestLog.HeadRevision, status.Entry.Revision, "TestUpdate2.04");
        }

        #endregion

    }
}
