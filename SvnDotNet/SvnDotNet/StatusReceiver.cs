using System;
using System.Collections.Generic;
using System.Text;
using PumaCode.SvnDotNet.SubversionSharp;

namespace PumaCode.SvnDotNet {
    internal class StatusReceiver {
        private readonly List<Status> _result;

        internal StatusReceiver()
        {
            _result = new List<Status>();
        }

        internal List<Status> Result
        {
            get { return _result; }
        }

        internal void Callback(IntPtr baton, SvnPath path, SvnWcStatus2 wcStatus)
        {
            Status status = Status.FromNative(wcStatus, path);
            _result.Add(status);
        }

    }
}
