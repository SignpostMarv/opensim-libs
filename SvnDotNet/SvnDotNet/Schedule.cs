using System;
using System.Collections.Generic;
using System.Text;

namespace PumaCode.SvnDotNet {
    /// <summary>
    /// Scheduled action (add, delete, replace ...)
    /// </summary>
    public enum Schedule {
        // This enum must stay in sync with SubversionSharp.SvnWcEntry.WcSchedule
        Normal,
        Add,
        Delete,
        Replace
    }
}
