using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.SubversionSharp {
    public unsafe class SvnVersion : IAprUnmanaged {
        private svn_version_t* _version;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_version_t {
            public int major;
            public int minor;
            public int patch;
            public IntPtr tag;
        }

        #region Generic embedding functions of an IntPtr
        public SvnVersion(IntPtr ptr)
        {
            _version = (svn_version_t*) ptr.ToPointer();
        }

        public static SvnVersion NoVersion = new SvnVersion(IntPtr.Zero);

        public bool IsNoVersion
        {
            get
            {
                return (_version == null);
            }
        }

        public bool IsNull
        {
            get
            {
                return (_version == null);
            }
        }

        private void CheckPtr()
        {
            if(_version == null)
                throw new SvnNullReferenceException();
        }

        public void ClearPtr()
        {
            _version = null;
        }

        public IntPtr ToIntPtr()
        {
            if(IsNoVersion)
                return IntPtr.Zero;
            else
                return new IntPtr(_version);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }
        public static implicit operator IntPtr(SvnVersion version)
        {
            if(version.IsNoVersion)
                return IntPtr.Zero;
            else
                return new IntPtr(version._version);
        }

        public static implicit operator SvnVersion(IntPtr ptr)
        {
            return new SvnVersion(ptr);
        }

        public override string ToString()
        {
            return ("[svn_version_t:" + (new IntPtr(_version)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper properties

        public int Major
        {
            get
            {
                CheckPtr();
                return (_version->major);
            }
        }

        public int Minor
        {
            get
            {
                CheckPtr();
                return (_version->minor);
            }
        }


        public int Patch
        {
            get
            {
                CheckPtr();
                return (_version->patch);
            }
        }

        public AprString Tag
        {
            get
            {
                CheckPtr();
                return (_version->tag);
            }
        }

        public string Version
        {
            get { return Major.ToString() + '.' + Minor + '.' + Patch; }
        }

        #endregion

    }
}
