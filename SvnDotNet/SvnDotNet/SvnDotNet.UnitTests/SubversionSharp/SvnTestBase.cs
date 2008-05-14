using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using NUnit.Framework;

using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

using PumaCode.SvnDotNet.AprSharp;
using PumaCode.SvnDotNet.SubversionSharp;

namespace PumaCode.SvnDotNet.UnitTests.SubversionSharp {
    // Helper methods for use in conducting unit tests

    public class SvnTestBase {
        /// <summary>
        /// The original URI of the repo before it is actually relocated to its full url
        /// </summary>
        protected const string TestRepoFakeRootUri = @"file:///svn/repo";

        // # of ticks since 1970-01-01: new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks
        private const long EpochTicks = 621355968000000000L;

        public const int TestWcBaseRevision = 6;

        private const string TestSvnRepoName = "svn-repo";
        private const string TestSvnRepoTarball = "svn-repo.tar.gz";
        protected const string TestUsername = "Test Username";

        private static string _testRepoDataDir;
        private static string _testRepoAbsoluteUri;
        private static string _testWcDir;

        private static uint _nextTempFilePrefix = 1;
        private static SvnTestLog _svnTestLog;
        private static LogEntry _wcBaseLogEntry;

        #region Member Variables
        private readonly TestDataStore _testDataStore
            = new TestDataStore();
        private SvnWcStatus2 _testStatus;
        private SvnInfo _testSvnInfo;
        private string _wcStatusPath;
        private string _svnInfoPath;

        #endregion

        #region Test Repo Helpers
        // These methods are used to un-tar the svn repo we use for testing. We don't
        // want to do this for each test class run, so the static GetTestRepo()
        // method is used in the TestFixtureSetup methods

        private static string _testBaseDataDirectory = null;
        private static string _testTempDirectory = null;

        protected void GetTestRepo(bool relocateToAbsoluteUri)
        {
            if(_testBaseDataDirectory == null)
                UntarTestRepo();

            if(_testRepoDataDir == null)
                _testRepoDataDir = PathCombine(_testBaseDataDirectory, "repo");

            if(_testWcDir == null)
                _testWcDir = PathCombine(_testBaseDataDirectory, "wc");

            if(relocateToAbsoluteUri && TestRepoAbsoluteUri == null) {
                // The actual URI of the test repo depends on where it happens to be located on
                // the user's system, so we need to retrieve that new URI and Relocate the WC to
                // that URI.
                Uri newWcUri = new Uri(TestRepoDataDir, UriKind.Absolute);

                SvnClient svnClient = new SvnClient();

                if(!newWcUri.IsFile)
                    throw new InvalidOperationException(
                        String.Format("Could not get new Uri of test repo '{0}'", TestRepoDataDir));

                TestRepoAbsoluteUri = newWcUri.AbsoluteUri;

                // We can't relocate while wc needs cleanup, and since the tests run after setup
                // method, we'll go ahead and test cleanup now.
                string checkPath = TestWcPath("2");
                SvnWcStatus2 status = GetSingleWcStatus(checkPath);
                Assert.IsTrue(status.Locked, "GetTestRepo.01");
                
                svnClient.Cleanup(_testWcDir);

                status = GetSingleWcStatus(checkPath);
                Assert.IsFalse(status.Locked, "GetTestRepo.02");

                //TODO: add assertions here to ensure relocate works
                svnClient.Relocate(_testWcDir, TestRepoFakeRootUri, _testRepoAbsoluteUri, true);
            }

        }

        private static void UntarTestRepo()
        {
            string myDir = Directory.GetCurrentDirectory();

            //TODO: we may need to change this if locking prevents easy delete and recreate
            string targetDir = PathCombine(myDir, TestSvnRepoName);

            try {
                // .NET Framework has no option to ignore read-only flags on delete,
                // so we need to brute-force it
                DirectoryInfo dir = new DirectoryInfo(targetDir);
                foreach(FileInfo fi in dir.GetFiles("*", SearchOption.AllDirectories)) {
                    if((fi.Attributes & FileAttributes.ReadOnly) != 0)
                        fi.Attributes -= FileAttributes.ReadOnly;
                }

                Directory.Delete(targetDir, true);
            }
            catch(DirectoryNotFoundException) {

            }
            catch(IOException) {
                Assert.Fail("Could not delete test repo directory '{0}'", targetDir);
            }

            Directory.CreateDirectory(targetDir);

            using(Stream inStream = new GZipInputStream(
                GetTestResourceStream(TestSvnRepoTarball))) {

                TarArchive archive = TarArchive.CreateInputTarArchive(inStream);

                archive.ExtractContents(targetDir);
                archive.CloseArchive();
            }

            _testBaseDataDirectory = targetDir;
            _testTempDirectory = PathCombine(targetDir, "tmp");
        }

        internal static AprFile GetTempAprFile(AprPool pool)
        {
            string tempFilePath = PathCombine(_testTempDirectory,
                Convert.ToString(_nextTempFilePrefix) + ".tmp");

            AprFile aprFile = AprFile.Open(tempFilePath,
                AprFile.Flags.Create | AprFile.Flags.Write,
                AprFile.Perms.OSDefault, pool);

            _nextTempFilePrefix++;
            return aprFile;
        }

        /// <summary>
        /// Returns a Stream for the specified resource.
        /// </summary>
        /// <param name="resourceName">The name of the resource, without the namespace</param>
        /// <returns>A System.IO.Stream of the specified resource.</returns>
        protected internal static Stream GetTestResourceStream(string resourceName)
        {
            Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(SvnTestBase), "Resources." + resourceName);

            if(resourceStream == null)
                throw new InvalidOperationException(string.Format(
                    "Could not get a resource stream for '{0}'", resourceName));

            return resourceStream;
        }

        /// <summary>
        /// Returns a string of the entire contents of the specified resource.
        /// </summary>
        /// <param name="resourceName">The name of the resource, without the namespace</param>
        /// <returns>A string containing the resource data.</returns>
        protected static string GetTestResourceString(string resourceName)
        {
            StreamReader streamReader = new StreamReader(
                GetTestResourceStream(resourceName));

            return streamReader.ReadToEnd();
        }

        protected static string TimestampToDateString(long timestamp)
        {
            long ticks = timestamp * 10 + EpochTicks;

            DateTime date = new DateTime(ticks, DateTimeKind.Utc);
            return date.ToString(@"yyyy-MM-dd\THH:mm:ss.ffffff\Z");
        }

        protected SvnWcStatus2 GetSingleWcStatus(string path)
        {
            SvnClient client = new SvnClient();

            _wcStatusPath = path;
            _testStatus = null;

            client.Status2(path, SvnTestRevisions.Working, GetSingleStatusReceiver,
                IntPtr.Zero, false, true, false, false, true);

            return _testStatus;
        }

        protected SvnInfo GetSingleInfo(string url)
        {
            SvnClient client = new SvnClient();

            _svnInfoPath = url;
            _testSvnInfo = null;

            client.Info(url, SvnTestRevisions.Unspecified, SvnTestRevisions.Head,
                TestInfoReceiver, IntPtr.Zero, false);

            return _testSvnInfo;
        }

        protected static void SetUsernameAuthProvider(SvnClient client)
        {
            client.AddPromptProvider(GetSimplePrompt, IntPtr.Zero, 0);
            client.OpenAuth();
        }

        private void GetSingleStatusReceiver(IntPtr baton, SvnPath path, SvnWcStatus2 status)
        {
            // When getting svnStatus of a directory, svnStatus of entries are returned as well
            // but we don't care about those

            if(path.Value == _wcStatusPath)
                _testStatus = status;
        }

        private SvnError TestInfoReceiver(IntPtr baton, SvnPath path, SvnInfo info, AprPool pool)
        {
            if(Regex.IsMatch(_svnInfoPath, "/" + path.Value + "$"))
                _testSvnInfo = info;
            return SvnError.NoError;
        }

        private static SvnError GetSimplePrompt(out SvnAuthCredUsername cred, IntPtr baton,
            AprString realm, bool maySave, AprPool pool)
        {
            cred = SvnAuthCredUsername.Alloc(pool);
            cred.Username = new AprString(TestUsername, pool);

            return SvnError.NoError;
        }

        /// <summary>
        /// The full url to the directory in which the test repository data files
        /// were extracted.
        /// </summary>
        protected static string TestRepoDataDir
        {
            get { return _testRepoDataDir; }
            set { _testRepoDataDir = value; }
        }

        /// <summary>
        /// The file:// URI which refers to the full url to the testing repository.
        /// </summary>
        protected static string TestRepoAbsoluteUri
        {
            get { return _testRepoAbsoluteUri; }
            set { _testRepoAbsoluteUri = value; }
        }

        /// <summary>
        /// The full url to the root directory of the testing working copy.
        /// </summary>
        protected static string TestWcDir
        {
            get { return _testWcDir; }
            set { _testWcDir = value; }
        }

        protected internal TestDataStore TestDataStore
        {
            get { return _testDataStore; }
        }

        /// <summary>
        /// An object containing all the expected log entry values
        /// </summary>
        protected static SvnTestLog SvnTestLog
        {
            get
            {
                if(_svnTestLog == null) {
                    if(_testBaseDataDirectory == null)
                        throw new InvalidOperationException(
                            "Can't get test log before unpacking test repo");

                    _svnTestLog = SvnTestLog.GetTestLog(PathCombine(_testBaseDataDirectory, "SvnLog.xml"));
                }

                return _svnTestLog;
            }
        }

        protected static LogEntry WcBaseLogEntry
        {
            get
            {
                if(_wcBaseLogEntry == null)
                    _wcBaseLogEntry = SvnTestLog.GetLogEntry(TestWcBaseRevision);

                return _wcBaseLogEntry;
            }
        }

        /// <summary>
        /// The directory where the test repository, working copy, temp dir, etc. were extracted
        /// </summary>
        public static string TestBaseDataDirectory
        {
            get { return _testBaseDataDirectory; }
        }

        /// <summary>
        /// The testing directory for tempfiles
        /// </summary>
        public static string TestTempDirectory
        {
            get { return _testTempDirectory; }
        }

        /// <summary>
        /// Returns the specified path below the working directory, canonicalized for the
        /// current platform
        /// </summary>
        protected static string TestWcPath(string path)
        {
            return (_testWcDir + "/" + path).Replace('/', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Combines the various path elements into a single path
        /// </summary>
        protected static string PathCombine(params string[] parts)
        {
            return PathCombine(null, parts);
        }

        private static string PathCombine(string prefix, IEnumerable<string> parts)
        {
            StringBuilder sb = new StringBuilder(prefix);
            bool first = (prefix == null); // if prefix was non-null, we already have our first part

            foreach(string part in parts) {
                if(!first)
                    sb.Append(Path.DirectorySeparatorChar);

                sb.Append(part);
                first = false;
            }

            return sb.ToString();

        }

        /// <summary>
        /// Reads the contents of two StreamReaders line-by-line and calls Assert.AreEqual on
        /// each line. Both StreamReaders are then closed.
        /// </summary>
        /// <param name="expectedReader">The StreamReader containing the expected results</param>
        /// <param name="actualReader">The StreamReader containing the actual results</param>
        /// <param name="skipDiffHeader">True if streams contain diff data which should skip
        /// the "header" during comparison</param>
        /// <param name="testName">The name of the test, for output in the failure Message if necessary</param>
        internal static void CompareStreams(StreamReader expectedReader, StreamReader actualReader, 
            bool skipDiffHeader, string testName)
        {
            bool foundDiffData = !skipDiffHeader;
            string actual, expected;

            uint line = 0;

            while(true) {
                if(actualReader.EndOfStream || expectedReader.EndOfStream) {
                    if(line == 0)
                        Assert.Fail("{0}: Actual, expected EOF were '{1}', '{2}' after comparing 0 lines",
                        testName, actualReader.EndOfStream, expectedReader.EndOfStream);

                    // neither reader should have leftover data
                    Assert.IsTrue(actualReader.EndOfStream && expectedReader.EndOfStream,
                        "{0}: Actual EOF was '{1}' while expected EOF was '{2}' after comparing '{3}' lines",
                        testName, actualReader.EndOfStream, expectedReader.EndOfStream, line);
                    break;
                }

                actual = actualReader.ReadLine();
                line++;

                if(!foundDiffData) {
                    if(actual.StartsWith("+++"))
                        foundDiffData = true;

                    continue;
                }

                expected = expectedReader.ReadLine();
                Assert.AreEqual(expected, actual, String.Format("{0}, Line {1}", testName, line));

                if(actual != expected)
                    break;
            }

            actualReader.Close();
            expectedReader.Close();

        }

        #endregion

        #region SvnTestRevisions
        // Help keep from having to create new SvnRevision objects for each test

        internal static class SvnTestRevisions {
            private static readonly SvnRevision _unspecified
                = new SvnRevision(Svn.Revision.Unspecified);
            private static readonly SvnRevision _head
                = new SvnRevision(Svn.Revision.Head);
            private static readonly SvnRevision _base
                = new SvnRevision(Svn.Revision.Base);
            private static readonly SvnRevision _committed
                = new SvnRevision(Svn.Revision.Committed);
            private static readonly SvnRevision _prev
                = new SvnRevision(Svn.Revision.Previous);
            private static readonly SvnRevision _working
                = new SvnRevision(Svn.Revision.Working);
            private static readonly SvnRevision _zero
                = new SvnRevision(0);

            public static SvnRevision Unspecified
            {
                get { return _unspecified; }
            }

            public static SvnRevision Head
            {
                get { return _head; }
            }

            public static SvnRevision Base
            {
                get { return _base; }
            }

            public static SvnRevision Committed
            {
                get { return _committed; }
            }

            public static SvnRevision Prev
            {
                get { return _prev; }
            }

            public static SvnRevision Working
            {
                get { return _working; }
            }

            public static SvnRevision Zero
            {
                get { return _zero; }
            }
        }

        #endregion

        #region SvnTestDiffOptions

        internal static class SvnTestDiffOptions {
            private static readonly List<string> _none
                = new List<string>();

            public static List<string> None
            {
                get { return _none; }
            }


        }

        #endregion

    }

    #region TestDataStore
    public class TestDataStore {
        private readonly List<string> _testData = new List<string>();

        public void ClearData()
        {
            _testData.Clear();
        }

        public void Add(string item)
        {
            _testData.Add(item);
        }

        public void AddFormat(string format, params object[] args)
        {
            _testData.Add(string.Format(format, args));
        }

        public string GetSortedData()
        {
            StringBuilder stringBuilder = new StringBuilder();
            _testData.Sort();

            foreach(string testLine in _testData) {
                stringBuilder.Append(testLine);
            }

            return stringBuilder.ToString();
        }
    }
    #endregion

}
