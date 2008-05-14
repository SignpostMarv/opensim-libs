using System;
using System.Collections.Generic;
using System.Text;

using PumaCode.SvnDotNet.SubversionSharp;

namespace PumaCode.SvnDotNet {
    /// <summary>
    /// Represents a Subversion revision. Implicit operators exist to allow using
    /// System.Int32 or System.DateTime in place of a Revision, but not vice versa.
    /// </summary>
    public class Revision {
        private readonly SvnRevision _revision;

        /// <summary>
        /// A revision explicitly identified by its revision number.
        /// </summary>
        public Revision(int revisionNumber)
        {
            _revision = new SvnRevision(revisionNumber);
        }

        /// <summary>
        /// A revision implicitly identified by a specific point in time.
        /// </summary>
        /// <param name="referenceDate"></param>
        public Revision(DateTime referenceDate)
        {
            _revision = new SvnRevision(Conversions.DateTimeToAprDate(referenceDate));
        }

        internal Revision(SvnRevision revision)
        {
            _revision = revision;
        }

        public static implicit operator Revision(DateTime date)
        {
            return new Revision(date);
        }

        public static implicit operator Revision(int revisionNumber)
        {
            return new Revision(revisionNumber);
        }

        public static Revision Head
        {
            get { return new Revision(Svn.Revision.Head); }
        }

        public static Revision Unspecified
        {
            get { return new Revision(Svn.Revision.Unspecified); }
        }

        internal SvnRevision SvnRevision
        {
            get { return _revision; }
        }
    }
}
