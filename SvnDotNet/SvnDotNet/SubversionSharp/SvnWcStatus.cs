//  SubversionSharp, a wrapper library around the Subversion client API
#region Copyright (C) 2004 SOFTEC sa.
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//  Sources, support options and lastest version of the complete library
//  is available from:
//		http://www.softec.st/SubversionSharp
//		Support@softec.st
//
//  Initial authors : 
//		Denis Gervalle
//		Olivier Desaive
#endregion
//
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.SubversionSharp {
    public unsafe class SvnWcStatus : IAprUnmanaged {
        public delegate void Func(IntPtr baton, SvnPath path, SvnWcStatus status);

        public enum Kind {
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

        private svn_wc_status_t* _status;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_wc_status_t {
            public IntPtr entry;
            public int text_status;
            public int prop_status;
            public int locked;
            public int copied;
            public int switched;
            public int repos_text_status;
            public int repos_prop_status;
        }

        #region Generic embedding functions of an IntPtr
        public SvnWcStatus(IntPtr ptr)
        {
            _status = (svn_wc_status_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (_status == null);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            _status = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(_status);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnWcStatus status)
        {
            return new IntPtr(status._status);
        }

        public static implicit operator SvnWcStatus(IntPtr ptr)
        {
            return new SvnWcStatus(ptr);
        }

        public override string ToString()
        {
            return ("[svn_wc_status_t:" + (new IntPtr(_status)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Properties wrappers
        public SvnWcEntry Entry
        {
            get
            {
                CheckPtr();
                return (_status->entry);
            }
        }

        public Kind TextStatus
        {
            get
            {
                CheckPtr();
                return ((Kind) _status->text_status);
            }
        }

        public Kind PropStatus
        {
            get
            {
                CheckPtr();
                return ((Kind) _status->prop_status);
            }
        }

        public bool Locked
        {
            get
            {
                CheckPtr();
                return (_status->locked != 0);
            }
        }

        public bool Copied
        {
            get
            {
                CheckPtr();
                return (_status->copied != 0);
            }
        }

        public bool Switched
        {
            get
            {
                CheckPtr();
                return (_status->switched != 0);
            }
        }

        public Kind ReposTextStatus
        {
            get
            {
                CheckPtr();
                return ((Kind) _status->repos_text_status);
            }
        }

        public Kind ReposPropStatus
        {
            get
            {
                CheckPtr();
                return ((Kind) _status->repos_prop_status);
            }
        }
        #endregion
    }

    public unsafe class SvnWcStatus2 : IAprUnmanaged {
        public delegate void Func(IntPtr baton, SvnPath path, SvnWcStatus2 status);

        private svn_wc_status2_t* mStatus;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_wc_status2_t {
            public IntPtr entry;
            public int text_status;
            public int prop_status;
            public int locked;
            public int copied;
            public int switched;
            public int repos_text_status;
            public int repos_prop_status;
            public IntPtr repos_lock;
        }

        #region Generic embedding functions of an IntPtr
        public SvnWcStatus2(IntPtr ptr)
        {
            mStatus = (svn_wc_status2_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (mStatus == null);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            mStatus = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(mStatus);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnWcStatus2 status)
        {
            return new IntPtr(status.mStatus);
        }

        public static implicit operator SvnWcStatus2(IntPtr ptr)
        {
            return new SvnWcStatus2(ptr);
        }

        public override string ToString()
        {
            return ("[svn_wc_status2_t:" + (new IntPtr(mStatus)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Properties wrappers
        public SvnWcEntry Entry
        {
            get
            {
                CheckPtr();
                return (mStatus->entry);
            }
        }

        public SvnWcStatus.Kind TextStatus
        {
            get
            {
                CheckPtr();
                return ((SvnWcStatus.Kind) mStatus->text_status);
            }
        }

        public SvnWcStatus.Kind PropStatus
        {
            get
            {
                CheckPtr();
                return ((SvnWcStatus.Kind) mStatus->prop_status);
            }
        }

        public bool Locked
        {
            get
            {
                CheckPtr();
                return (mStatus->locked != 0);
            }
        }

        public bool Copied
        {
            get
            {
                CheckPtr();
                return (mStatus->copied != 0);
            }
        }

        public bool Switched
        {
            get
            {
                CheckPtr();
                return (mStatus->switched != 0);
            }
        }

        public SvnWcStatus.Kind ReposTextStatus
        {
            get
            {
                CheckPtr();
                return ((SvnWcStatus.Kind) mStatus->repos_text_status);
            }
        }

        public SvnWcStatus.Kind ReposPropStatus
        {
            get
            {
                CheckPtr();
                return ((SvnWcStatus.Kind) mStatus->repos_prop_status);
            }
        }

        public SvnLock ReposLock
        {
            get
            {
                CheckPtr();
                return (mStatus->repos_lock);
            }
        }
        #endregion
    }

}
