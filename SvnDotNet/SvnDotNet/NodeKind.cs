using System;
using System.Collections.Generic;
using System.Text;

namespace PumaCode.SvnDotNet {
    /// <summary>
    /// Node kind (file, dir, ...)
    /// </summary>
    public enum NodeKind {
        // This enum must stay in sync with SubversionSharp.Svn.NodeKind
        None,
        File,
        Dir,
        Unknown
    }
}
