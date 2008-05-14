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
    public unsafe class SvnWcNotify : IAprUnmanaged {
        private svn_wc_notify_t* _notify;

#if WIN32
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
#else
		[StructLayout( LayoutKind.Sequential, Pack=4 )]
#endif
        private struct svn_wc_notify_t {
            public IntPtr path;
            public int action;
            public int kind;
            public IntPtr mime_type;
            public IntPtr svnlock;
            public IntPtr err;
            public int content_state;
            public int prop_state;
            public int lock_state;
            public int revision;
        }

        #region Enums
        public enum Actions {
            Add = 0,
            Copy,
            Delete,
            Restore,
            Revert,
            FailedRevert,
            Resolved,
            Skip,
            UpdateDelete,
            UpdateAdd,
            UpdateUpdate,
            UpdateCompleted,
            UpdateExternal,
            StatusCompleted,
            StatusExternal,
            CommitModified,
            CommitAdded,
            CommitDeleted,
            CommitReplaced,
            PostfixTxdelta,
            BlameRevision,
            Locked,
            Unlocked,
            FailedLock,
            FailedUnlock
        }

        public enum State {
            Inapplicable = 0,
            Unknown,
            Unchanged,
            Missing,
            Obstructed,
            Changed,
            Merged,
            Conflicted
        }

        public enum LockStates {
            Inapplicable = 0,
            Unknown,
            /** The lock wasn't changed. */
            Unchanged,
            /** The item was locked. */
            Locked,
            /** The item was unlocked. */
            Unlocked
        }
        #endregion

        public delegate void Func(IntPtr baton, SvnPath Path,
            Actions action, Svn.NodeKind kind,
            AprString mimeType, State contentState,
            State propState, int revNum);

        public delegate void Func2(IntPtr baton, SvnWcNotify notify, AprPool pool);


        #region Generic embedding functions of an IntPtr
        public SvnWcNotify(IntPtr ptr)
        {
            _notify = (svn_wc_notify_t*) ptr.ToPointer();
        }

        public bool IsNoNotify
        {
            get
            {
                return (_notify == null);
            }
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public bool IsNull
        {
            get
            {
                return (_notify == null);
            }
        }

        public void ClearPtr()
        {
            _notify = null;
        }

        public IntPtr ToIntPtr()
        {
            if(IsNoNotify)
                return IntPtr.Zero;
            else
                return new IntPtr(_notify);
        }

        private void CheckPtr()
        {
            if(_notify == null)
                throw new SvnNullReferenceException();
        }

        public static implicit operator IntPtr(SvnWcNotify notify)
        {
            if(notify.IsNoNotify)
                return IntPtr.Zero;
            else
                return new IntPtr(notify._notify);
        }

        public static implicit operator SvnWcNotify(IntPtr ptr)
        {
            return new SvnWcNotify(ptr);
        }

        public override string ToString()
        {
            return ("[svn_wc_notify_t:" + (new IntPtr(_notify)).ToInt32().ToString("X") + "]");
        }

        #endregion

        #region Wrapper properties
        public SvnPath Path
        {
            get
            {
                CheckPtr();
                return (_notify->path);
            }
        }

        public Actions Action
        {
            get
            {
                CheckPtr();
                return ((Actions) _notify->action);
            }
        }

        public Svn.NodeKind Kind
        {
            get
            {
                CheckPtr();
                return ((Svn.NodeKind) _notify->kind);
            }
        }

        public AprString MimeType
        {
            get
            {
                CheckPtr();
                return (_notify->mime_type);
            }
        }

        public SvnLock Lock
        {
            get
            {
                CheckPtr();
                return (new SvnLock(_notify->svnlock));
            }
        }

        public SvnError Err
        {
            get
            {
                CheckPtr();
                return (new SvnError(_notify->err));
            }
        }

        public State ContentState
        {
            get
            {
                CheckPtr();
                return ((State) _notify->content_state);
            }
        }

        public State PropState
        {
            get
            {
                CheckPtr();
                return ((State) _notify->prop_state);
            }
        }

        public LockStates LockState
        {
            get
            {
                CheckPtr();
                return ((LockStates) _notify->lock_state);
            }
        }

        public int Revision
        {
            get
            {
                CheckPtr();
                return (_notify->revision);
            }
        }

        #endregion
    }
}