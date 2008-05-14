using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.SubversionSharp {
    /// <summary>
    /// Summary description for SvnInfo.
    /// </summary>
    public unsafe class SvnInfo : IAprUnmanaged {
        private svn_info_t* _info;
#if WIN32
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
#else
		[StructLayout( LayoutKind.Sequential, Pack=4 )]
#endif
        private struct svn_info_t {
            public IntPtr URL;
            public int rev;
            public int kind;
            public IntPtr repos_root_URL;
            public IntPtr repos_UUID;
            public int last_changed_rev;
            public long last_changed_date;
            public IntPtr last_changed_author;
            public IntPtr Lock;
            public int has_wc_info;
            public int schedule;
            public IntPtr copyfrom_url;
            public int copyfrom_rev;
            public long text_time;
            public long prop_time;
            public IntPtr checksum;
            public IntPtr conflict_old;
            public IntPtr conflict_new;
            public IntPtr conflict_wrk;
            public IntPtr prejfile;
        }

        #region Generic embedding functions of an IntPtr
        public SvnInfo(IntPtr ptr)
        {
            _info = (svn_info_t*) ptr.ToPointer();
        }

        public static SvnInfo NoInfo = new SvnInfo(IntPtr.Zero);

        public bool IsNoInfo
        {
            get
            {
                return (_info == null);
            }
        }

        public bool IsNull
        {
            get
            {
                return (_info == null);
            }
        }

        private void CheckPtr()
        {
            if(_info == null)
                throw new SvnNullReferenceException();
        }

        public void ClearPtr()
        {
            _info = null;
        }

        public IntPtr ToIntPtr()
        {
            if(IsNoInfo)
                return IntPtr.Zero;
            else
                return new IntPtr(_info);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnInfo info)
        {
            if(info.IsNoInfo)
                return IntPtr.Zero;
            else
                return new IntPtr(info._info);
        }

        public static implicit operator SvnInfo(IntPtr ptr)
        {
            return new SvnInfo(ptr);
        }

        public override string ToString()
        {
            return ("[svn_info_t:" + (new IntPtr(_info)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper properties
        public SvnUrl URL
        {
            get
            {
                CheckPtr();
                return (_info->URL);
            }
        }

        public int Rev
        {
            get
            {
                CheckPtr();
                return _info->rev;
            }
        }

        public Svn.NodeKind Kind
        {
            get
            {
                CheckPtr();
                return (Svn.NodeKind) _info->kind;
            }
        }

        public SvnUrl ReposRootURL
        {
            get
            {
                CheckPtr();
                return (_info->repos_root_URL);
            }
        }

        public AprString ReposUUID
        {
            get
            {
                CheckPtr();
                return (_info->repos_UUID);
            }
        }

        public int LastChangedRev
        {
            get
            {
                CheckPtr();
                return (_info->last_changed_rev);
            }
        }

        public long LastChangedDate
        {
            get
            {
                CheckPtr();
                return (_info->last_changed_date);
            }
        }

        public SvnData LastChangedAuthor
        {
            get
            {
                CheckPtr();
                return (_info->last_changed_author);
            }
        }

        public SvnLock Lock
        {
            get
            {
                CheckPtr();
                return (_info->Lock);
            }
        }

        public bool HasWcInfo
        {
            get
            {
                CheckPtr();
                return (_info->has_wc_info != 0);
            }
        }

        public SvnWcEntry.WcSchedule Schedule
        {
            get
            {
                CheckPtr();
                return ((SvnWcEntry.WcSchedule) _info->schedule);
            }
        }

        public SvnUrl CopyFromUrl
        {
            get
            {
                CheckPtr();
                return (_info->copyfrom_url);
            }
        }

        public int CopyFromRev
        {
            get
            {
                CheckPtr();
                return (_info->copyfrom_rev);
            }
        }

        public long TextTime
        {
            get
            {
                CheckPtr();
                return (_info->text_time);
            }
        }

        public long PropTime
        {
            get
            {
                CheckPtr();
                return (_info->prop_time);
            }
        }

        public AprString Checksum
        {
            get
            {
                CheckPtr();
                return (_info->checksum);
            }
        }

        public SvnPath ConflictOld
        {
            get
            {
                CheckPtr();
                return (_info->conflict_old);
            }
        }

        public SvnPath ConflictNew
        {
            get
            {
                CheckPtr();
                return (_info->conflict_new);
            }
        }

        public SvnPath ConflictWrk
        {
            get
            {
                CheckPtr();
                return (_info->conflict_wrk);
            }
        }

        public SvnPath PrejFile
        {
            get
            {
                CheckPtr();
                return (_info->prejfile);
            }
        }
        #endregion
    }
}
