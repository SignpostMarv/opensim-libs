using System;
using System.Collections.Generic;
using System.Text;

namespace PumaCode.SvnDotNet {
    /// <summary>
    /// The type of svnStatus for a working copy entry.
    /// </summary>
    public enum StatusKind {
        // This enum must stay in sync with SubversionSharp.SvnWcStatus.Kind
        None = 1,
        Unversioned,
        Normal,
        Added,
        Missing,
        Deleted,
        Replaced,
        Modified,
        Merged,
        Conflicted,
        Ignored,
        Obstructed,
        External,
        Incomplete
    }
}
