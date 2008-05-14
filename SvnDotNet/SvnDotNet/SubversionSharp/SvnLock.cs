using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.SubversionSharp {
    /// <summary>
    /// Summary description for SvnLock.
    /// </summary>
    public unsafe class SvnLock : IAprUnmanaged {
        private svn_lock_t* _svnLock;

#if WIN32
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
#else
		[StructLayout( LayoutKind.Sequential, Pack=4 )]
#endif
        private struct svn_lock_t {
            public IntPtr path;             // the path this lock applies to 
            public IntPtr token;            // unique URI representing lock 
            public IntPtr owner;            // the username which owns the lock 
            public IntPtr comment;          // (optional) description of lock  
            public int is_dav_comment;				// was comment made by generic DAV client? 
            public long creation_date;				// when lock was made 
            public long expiration_date;			// (optional) when lock will expire;
            // If value is 0, lock will never expire. 
        }

        #region Generic embedding functions of an IntPtr
        public SvnLock(IntPtr ptr)
        {
            _svnLock = (svn_lock_t*) ptr.ToPointer();
        }

        public static SvnLock NoLock = new SvnLock(IntPtr.Zero);

        public bool IsNoLock
        {
            get
            {
                return (_svnLock == null);
            }
        }

        public bool IsNull
        {
            get
            {
                return (_svnLock == null);
            }
        }

        private void CheckPtr()
        {
            if(_svnLock == null)
                throw new SvnNullReferenceException();
        }

        public void ClearPtr()
        {
            _svnLock = null;
        }

        public IntPtr ToIntPtr()
        {
            if(IsNoLock)
                return IntPtr.Zero;
            else
                return new IntPtr(_svnLock);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnLock lck)
        {
            if(lck.IsNoLock)
                return IntPtr.Zero;
            else
                return new IntPtr(lck._svnLock);
        }

        public static implicit operator SvnLock(IntPtr ptr)
        {
            return new SvnLock(ptr);
        }

        public override string ToString()
        {
            return ("[svn_lock_t:" + (new IntPtr(_svnLock)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static SvnLock Create(AprPool pool)
        {
            return (new SvnLock(Svn.svn_lock_create(pool)));
        }

        public static SvnLock Dup(SvnLock lck, AprPool pool)
        {
            return (new SvnLock(Svn.svn_lock_dup(lck, pool)));
        }
        #endregion

        #region Wrapper properties
        public SvnPath Path
        {
            get
            {
                CheckPtr();
                return (_svnLock->path);
            }
        }

        public AprString Token
        {
            get
            {
                CheckPtr();
                return (_svnLock->token);
            }
        }

        public SvnData Owner
        {
            get
            {
                CheckPtr();
                return (_svnLock->owner);
            }
        }

        public SvnData Comment
        {
            get
            {
                CheckPtr();
                return (_svnLock->comment);
            }
        }

        public bool IsDavComment
        {
            get
            {
                CheckPtr();
                return (_svnLock->is_dav_comment != 0);
            }
        }

        public long CreationDate
        {
            get
            {
                CheckPtr();
                return (_svnLock->creation_date);
            }
        }

        public long ExpirationDate
        {
            get
            {
                CheckPtr();
                return (_svnLock->expiration_date);
            }
        }
        #endregion
    }
}
