using System;
using System.Collections.Generic;
using System.Text;
using PumaCode.SvnDotNet.SubversionSharp;

namespace PumaCode.SvnDotNet {
    /// <summary>
    /// Represents data returned from a Status call on a local working copy.
    /// </summary>
    /// <remarks>
    /// This class is generally analogous to the svn_client svn_wc_status2_t struct, see
    /// http://svn.collab.net/svn-doxygen/structsvn__wc__status2__t.html
    /// </remarks>
    public class Status {

        #region Member Variables
        private readonly Entry _entry;
        private readonly bool _isCopied;
        private readonly bool _isLocked;
        private readonly bool _isSwitched;
        private readonly string _path;
        private readonly StatusKind _propStatus;
        private readonly Lock _reposLock;
        private readonly StatusKind _reposPropStatus;
        private readonly StatusKind _reposTextStatus;
        private readonly StatusKind _textStatus; 
        #endregion

        #region Constructor and Factory Method
        private Status(SvnWcStatus2 svnStatus, SvnPath path)
        {
            _entry = Entry.FromNative(svnStatus.Entry);
            _isCopied = svnStatus.Copied;
            _isLocked = svnStatus.Locked;
            _isSwitched = svnStatus.Switched;
            _path = path.Value;
            _propStatus = (StatusKind) svnStatus.PropStatus;
            _reposLock = Lock.FromNative(svnStatus.ReposLock);
            _reposPropStatus = (StatusKind) svnStatus.ReposPropStatus;
            _reposTextStatus = (StatusKind) svnStatus.ReposTextStatus;
            _textStatus = (StatusKind) svnStatus.TextStatus;
        }

        internal static Status FromNative(SvnWcStatus2 svnStatus, SvnPath path)
        {
            return svnStatus.IsNull ? null : new Status(svnStatus, path);
        }

        #endregion

        /// <summary>
        /// The working copy entry for which this status was retrieved; can be null
        /// if not under version control.
        /// </summary>
        public Entry Entry
        {
            get { return _entry; }
        }

        /// <summary>
        /// A file or directory can be IsCopied if it's scheduled for
        /// addition-with-history (or part of a subtree that is scheduled as such).
        /// </summary>
        public bool IsCopied
        {
            get { return _isCopied; }
        }

        /// <summary>
        /// A directory can be IsLocked if a working copy update was interrupted. This
        /// has no relation to the concept of the repository lock operation which prevents
        /// updates to a resource; see <see cref="ReposLock"/> for that.
        /// </summary>
        public bool IsLocked
        {
            get { return _isLocked; }
        }

        /// <summary>
        /// A file or directory can be IsSwitched if the switch command has been used.
        /// </summary>
        public bool IsSwitched
        {
            get { return _isSwitched; }
        }

        /// <summary>
        /// The local path for which this status was retrieved.
        /// </summary>
        public string Path
        {
            get { return _path; }
        }

        /// <summary>
        /// The svnStatus of the entry's versioned properties.
        /// </summary>
        public StatusKind PropStatus
        {
            get { return _propStatus; }
        }

        /// <summary>
        /// The entry's lock in the repository, if any.
        /// </summary>
        public Lock ReposLock
        {
            get
            {
                return _reposLock;
            }
        }

        /// <summary>
        /// The svnStatus of the entry's versioned properties in the repository.
        /// </summary>
        public StatusKind ReposPropStatus
        {
            get { return _reposPropStatus; }
        }

        /// <summary>
        /// The svnStatus of the entry's text (contents) in the repository.
        /// </summary>
        public StatusKind ReposTextStatus
        {
            get { return _reposTextStatus; }
        }

        /// <summary>
        /// The svnStatus of the entry's text (contents).
        /// </summary>
        public StatusKind TextStatus
        {
            get { return _textStatus; }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Path, TextStatus);
        }

    }
}
