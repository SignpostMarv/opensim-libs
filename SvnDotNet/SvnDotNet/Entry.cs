using System;
using System.Collections.Generic;
using System.Text;
using PumaCode.SvnDotNet.SubversionSharp;

namespace PumaCode.SvnDotNet {
    /// <summary>
    /// A working copy entry -- that is, revision control information about one versioned entity.
    /// </summary>
    /// <remarks>
    /// This class is generally analogous to the svn_client svn_wc_entry_t struct, see
    /// http://svn.collab.net/svn-doxygen/structsvn__wc__entry__t.html
    /// </remarks>
    public class Entry {

        #region Member Variables
        private readonly string _checksum;
        private readonly string _commitAuthor;
        private readonly DateTime _commitDate;
        private readonly int _commitRevision;
        private readonly string _conflictNew;
        private readonly string _conflictOld;
        private readonly string _conflictWorking;
        private readonly int _copyFromRevision;
        private readonly string _copyFromUrl;
        private readonly bool _isAbsent;
        private readonly bool _isCopied;
        private readonly bool _isDeleted;
        private readonly bool _isIncomplete;
        private readonly NodeKind _nodeKind;
        private readonly string _lockComment;
        private readonly DateTime _lockCreationDate;
        private readonly string _lockOwner;
        private readonly string _lockToken;
        private readonly string _name;
        private readonly DateTime _propTime;
        private readonly string _propertyRejectFile;
        private readonly string _repositoryUrl;
        private readonly int _revision;
        private readonly Schedule _schedule;
        private readonly DateTime _textTime;
        private readonly string _url;
        private readonly string _repositoryUuid;
        #endregion

        #region Constructor and Factory Method
        private Entry(SvnWcEntry svnEntry)
        {
            _isAbsent = svnEntry.Absent;
            _checksum = svnEntry.CheckSum.Value;
            _commitAuthor = Conversions.SvnDataToString(svnEntry.CommitAuthor);
            _commitDate = Conversions.AprDateToDateTime(svnEntry.CommitDate);
            _commitRevision = svnEntry.CommitRev;
            _conflictNew = svnEntry.ConflictNew.Value;
            _conflictOld = svnEntry.ConflictOld.Value;
            _conflictWorking = svnEntry.ConflictWork.Value;
            _isCopied = svnEntry.Copied;
            _copyFromRevision = svnEntry.CopyFromRevision;
            _copyFromUrl = svnEntry.CopyFromUrl.Value;
            _isDeleted = svnEntry.Deleted;
            _isIncomplete = svnEntry.Incomplete;
            _nodeKind = (NodeKind) svnEntry.Kind;
            _lockComment = Conversions.SvnDataToString(svnEntry.LockComment);
            _lockCreationDate = Conversions.AprDateToDateTime(svnEntry.LockCreationDate);
            _lockOwner = Conversions.SvnDataToString(svnEntry.LockOwner);
            _lockToken = Conversions.SvnDataToString(svnEntry.LockToken);
            _name = svnEntry.Name.Value;
            _propTime = Conversions.AprDateToDateTime(svnEntry.PropTime);
            _propertyRejectFile = svnEntry.RejectFile.Value;
            _repositoryUrl = svnEntry.Repos.Value;
            _revision = svnEntry.Revision;
            _schedule = (Schedule) svnEntry.Schedule;
            _textTime = Conversions.AprDateToDateTime(svnEntry.TextTime);
            _url = svnEntry.Url.Value;
            _repositoryUuid = svnEntry.Uuid.Value;
        }

        internal static Entry FromNative(SvnWcEntry svnEntry)
        {
            return svnEntry.IsNull ? null : new Entry(svnEntry);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Hex MD5 checksum for the untranslated text base file, can be null for backwards compatibility.
        /// </summary>
        public string Checksum
        {
            get { return _checksum; }
        }

        /// <summary>
        /// The last commit author of this item
        /// </summary>
        public string CommitAuthor
        {
            get { return _commitAuthor; }
        }

        /// <summary>
        /// Last date this item was changed
        /// </summary>
        public DateTime CommitDate
        {
            get { return _commitDate; }
        }

        /// <summary>
        /// Last revision this item was changed
        /// </summary>
        public int CommitRevision
        {
            get { return _commitRevision; }
        }

        /// <summary>
        /// Path to new version of conflicted file
        /// </summary>
        public string ConflictNew
        {
            get { return _conflictNew; }
        }

        /// <summary>
        /// Path to old version of conflicted file
        /// </summary>
        public string ConflictOld
        {
            get { return _conflictOld; }
        }

        /// <summary>
        /// Path to working version of conflicted file
        /// </summary>
        public string ConflictWorking
        {
            get { return _conflictWorking; }
        }

        /// <summary>
        /// If scheduled for copy, the source revision
        /// </summary>
        public int CopyFromRevision
        {
            get { return _copyFromRevision; }
        }

        /// <summary>
        /// If scheduled for copy, the source location
        /// </summary>
        public string CopyFromUrl
        {
            get { return _copyFromUrl; }
        }

        /// <summary>
        /// Entry is Absent -- we know an entry of this name exists, but that's all
        /// (usually this happens because of authz restrictions)
        /// </summary>
        public bool IsAbsent
        {
            get { return _isAbsent; }
        }

        /// <summary>
        /// Entry is in a Copied state (possibly because the entry is a child of a path
        /// that is scheduled for Add or Replace, when the entry itself is Normal)
        /// </summary>
        public bool IsCopied
        {
            get { return _isCopied; }
        }

        /// <summary>
        /// Deleted, but parent revision lags behind
        /// </summary>
        public bool IsDeleted
        {
            get { return _isDeleted; }
        }

        /// <summary>
        /// For directories, implies whole entries file is incomplete
        /// </summary>
        public bool IsIncomplete
        {
            get { return _isIncomplete; }
        }

        /// <summary>
        /// The entry's name
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Node kind (file, dir, ...)
        /// </summary>
        public NodeKind NodeKind
        {
            get { return _nodeKind; }
        }

        /// <summary>
        /// Lock comment, or null if not locked in this WC or no comment
        /// </summary>
        public string LockComment
        {
            get { return _lockComment; }
        }

        /// <summary>
        /// Lock creation date, or DateTime.MinValue if not locked in this WC
        /// </summary>
        public DateTime LockCreationDate
        {
            get { return _lockCreationDate; }
        }

        /// <summary>
        /// Lock owner, or null if not locked in this WC
        /// </summary>
        public string LockOwner
        {
            get { return _lockOwner; }
        }

        /// <summary>
        /// Lock token, or null if path not locked in this WC
        /// </summary>
        public string LockToken
        {
            get { return _lockToken; }
        }

        /// <summary>
        /// Last up-to-date time for properties, or DateTime.MinValue if no information available
        /// </summary>
        public DateTime PropTime
        {
            get { return _propTime; }
        }

        /// <summary>
        /// Property reject file
        /// </summary>
        public string PropertyRejectFile
        {
            get { return _propertyRejectFile; }
        }

        /// <summary>
        /// Canonical repository URL, or null if not known
        /// </summary>
        public string RepositoryUrl
        {
            get { return _repositoryUrl; }
        }

        /// <summary>
        /// Base revision
        /// </summary>
        public int Revision
        {
            get { return _revision; }
        }

        /// <summary>
        /// Scheduled action (add, delete, replace ...)
        /// </summary>
        public Schedule Schedule
        {
            get { return _schedule; }
        }

        /// <summary>
        /// Last up-to-date time for text contents, or DateTime.MinValue if no information available
        /// </summary>
        public DateTime TextTime
        {
            get { return _textTime; }
        }

        /// <summary>
        /// URL in repository
        /// </summary>
        public string Url
        {
            get { return _url; }
        }

        /// <summary>
        /// Repository UUID
        /// </summary>
        public string RepositoryUuid
        {
            get { return _repositoryUuid; }
        }

        public override string ToString()
        {
            return _name;
        }

        #endregion
    }
}
