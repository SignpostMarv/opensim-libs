using System;
using System.Collections.Generic;
using System.Text;
using PumaCode.SvnDotNet.SubversionSharp;

namespace PumaCode.SvnDotNet {
    public class SubversionClient {

        private readonly SvnClient _client;

        public SubversionClient()
        {
            _client = new SvnClient();
        }

        /// <summary>
        /// Retrieve status of <paramref name="path"/> and its children.
        /// </summary>
        /// <param name="path">Path to a working copy directory or single file</param>
        public IList<Status> Status(string path)
        {
            int compareRevision;
            return Status(path, Revision.Unspecified, true, false, false, true, false, out compareRevision);
        }

        /// <summary>
        /// Retrieve status of <paramref name="path"/> and its children.
        /// </summary>
        /// <param name="path">Path to a working copy directory or single file</param>
        /// <param name="recurse">If true, recurse fully, else do only immediate children.</param>
        /// <param name="getAll">If true, retrieve all entries; otherwise, retrieve only 
        /// "interesting" entries (local mods and/or out-of-date).</param>
        public IList<Status> Status(string path, bool recurse, bool getAll)
        {
            int compareRevision;
            return Status(path, Revision.Unspecified, recurse, getAll, false, true,
                false, out compareRevision);
        }

        /// <summary>
        /// Retrieve status of <paramref name="path"/> and its children.
        /// </summary>
        /// <param name="path">Path to a working copy directory or single file</param>
        /// <param name="revision">Operative revision</param>
        /// <param name="recurse">If true, recurse fully, else do only immediate children.</param>
        /// <param name="getAll">If true, retrieve all entries; otherwise, retrieve only 
        /// "interesting" entries (local mods and/or out-of-date).</param>
        /// <param name="update">If is set, contact the repository and augment the
        /// svnStatus structures with information about out-of-dateness (with 
        /// respect to <paramref name="revision"/>). Also, returns the actual revision against which the
        /// working copy was compared.</param>
        /// <param name="useIgnore">If true, honor the client and working copy ignore patterns
        /// and don't retrieve matching files or directories.</param>
        /// <param name="ignoreExternals">If true, don't recurse into externals
        /// definitions (if any exist) after handling the main target.</param>
        /// <param name="compareRevision">If <paramref name="update"/> is set, will be set to
        /// the actual revision against which the working copy was compared. Otherwise -1.</param>
        /// <returns>A collection of StatusEntry objects</returns>
        public IList<Status> Status(string path, Revision revision, bool recurse, bool getAll,
            bool update, bool useIgnore, bool ignoreExternals, out int compareRevision)
        {
            StatusReceiver receiver = new StatusReceiver();

            compareRevision = _client.Status2(path, revision.SvnRevision, receiver.Callback,
                IntPtr.Zero, recurse, getAll, update, !useIgnore, ignoreExternals);

            return receiver.Result;
        }

    }
}