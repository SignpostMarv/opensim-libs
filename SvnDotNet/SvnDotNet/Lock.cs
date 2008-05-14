using System;
using System.Collections.Generic;
using System.Text;
using PumaCode.SvnDotNet.SubversionSharp;

namespace PumaCode.SvnDotNet {
    public class Lock {
        private readonly SvnLock _svnLock;

        private Lock(SvnLock svnLock)
        {
            _svnLock = svnLock;
        }

        internal static Lock FromNative(SvnLock svnLock)
        {
            return svnLock.IsNull ? null : new Lock(svnLock);
        }

        public string Comment
        {
            get { return Conversions.SvnDataToString(_svnLock.Comment); }
        }
    }
}
