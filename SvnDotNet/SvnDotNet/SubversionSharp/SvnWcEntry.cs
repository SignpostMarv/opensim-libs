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
    public unsafe class SvnWcEntry : IAprUnmanaged {
        public enum WcSchedule {
            Normal,
            Add,
            Delete,
            Replace
        }

        private svn_wc_entry_t* _entry;

#if WIN32
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
#else
		[StructLayout( LayoutKind.Sequential, Pack=4 )]
#endif
        private struct svn_wc_entry_t {
            public IntPtr name;
            public int revision;
            public IntPtr url;
            public IntPtr repos;
            public IntPtr uuid;
            public int kind;
            public int schedule;
            public int copied;
            public int deleted;
            public int absent;
            public int incomplete;
            public IntPtr copyfrom_url;
            public int copyfrom_rev;
            public IntPtr conflict_old;
            public IntPtr conflict_new;
            public IntPtr conflict_wrk;
            public IntPtr prejfile;
            public long text_time;
            public long prop_time;
            public IntPtr checksum;
            public int cmt_rev;
            public long cmt_date;
            public IntPtr cmt_author;

            // Locks
            /// <summary>
            /// lock token or NULL if path not locked in this WC 
            /// </summary>
            public IntPtr lock_token;
            /// <summary>
            /// lock owner, or NULL if not locked in this WC
            /// </summary>
            public IntPtr lock_owner;
            /// <summary>
            /// lock comment or NULL if not locked in this WC or no comment
            /// </summary>
            public IntPtr lock_comment;
            /// <summary>
            /// Lock creation date or 0 if not locked in this WC
            /// </summary>
            public long lock_creation_date;
        }

        #region Generic embedding functions of an IntPtr
        public SvnWcEntry(IntPtr ptr)
        {
            _entry = (svn_wc_entry_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (_entry == null);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            _entry = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(_entry);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnWcEntry entry)
        {
            return new IntPtr(entry._entry);
        }

        public static implicit operator SvnWcEntry(IntPtr ptr)
        {
            return new SvnWcEntry(ptr);
        }

        public override string ToString()
        {
            return ("[svn_wc_entry_t:" + (new IntPtr(_entry)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Properties wrappers
        public SvnPath Name
        {
            get
            {
                CheckPtr();
                return (_entry->name);
            }
        }

        public int Revision
        {
            get
            {
                CheckPtr();
                return (_entry->revision);
            }
        }

        public SvnUrl Url
        {
            get
            {
                CheckPtr();
                return (_entry->url);
            }
        }

        public SvnUrl Repos
        {
            get
            {
                CheckPtr();
                return (_entry->repos);
            }
        }

        public AprString Uuid
        {
            get
            {
                CheckPtr();
                return (_entry->uuid);
            }
        }

        public Svn.NodeKind Kind
        {
            get
            {
                CheckPtr();
                return ((Svn.NodeKind) _entry->kind);
            }
        }

        public WcSchedule Schedule
        {
            get
            {
                CheckPtr();
                return ((WcSchedule) _entry->schedule);
            }
        }

        public bool Copied
        {
            get
            {
                CheckPtr();
                return (_entry->copied != 0);
            }
        }

        public bool Deleted
        {
            get
            {
                CheckPtr();
                return (_entry->deleted != 0);
            }
        }

        public bool Absent
        {
            get
            {
                CheckPtr();
                return (_entry->absent != 0);
            }
        }

        public bool Incomplete
        {
            get
            {
                CheckPtr();
                return (_entry->incomplete != 0);
            }
        }

        public SvnUrl CopyFromUrl
        {
            get
            {
                CheckPtr();
                return (_entry->copyfrom_url);
            }
        }

        public int CopyFromRevision
        {
            get
            {
                CheckPtr();
                return (_entry->copyfrom_rev);
            }
        }

        public SvnPath ConflictOld
        {
            get
            {
                CheckPtr();
                return (_entry->conflict_old);
            }
        }

        public SvnPath ConflictNew
        {
            get
            {
                CheckPtr();
                return (_entry->conflict_new);
            }
        }

        public SvnPath ConflictWork
        {
            get
            {
                CheckPtr();
                return (_entry->conflict_wrk);
            }
        }

        public SvnPath RejectFile
        {
            get
            {
                CheckPtr();
                return (_entry->prejfile);
            }
        }

        public long TextTime
        {
            get
            {
                CheckPtr();
                return (_entry->text_time);
            }
        }

        public long PropTime
        {
            get
            {
                CheckPtr();
                return (_entry->prop_time);
            }
        }

        public AprString CheckSum
        {
            get
            {
                CheckPtr();
                return (_entry->checksum);
            }
        }

        public int CommitRev
        {
            get
            {
                CheckPtr();
                return (_entry->cmt_rev);
            }
        }

        public long CommitDate
        {
            get
            {
                CheckPtr();
                return (_entry->cmt_date);
            }
        }

        public SvnData CommitAuthor
        {
            get
            {
                CheckPtr();
                return (_entry->cmt_author);
            }
        }

        /// <summary>
        /// lock token or NULL if path not locked in this WC 
        /// </summary>
        public SvnData LockToken
        {
            get
            {
                CheckPtr();
                return (_entry->lock_token);
            }
        }
        /// <summary>
        /// lock owner, or NULL if not locked in this WC
        /// </summary>
        public SvnData LockOwner
        {
            get
            {
                CheckPtr();
                return (_entry->lock_owner);
            }
        }
        /// <summary>
        /// lock comment or NULL if not locked in this WC or no comment
        /// </summary>
        public SvnData LockComment
        {
            get
            {
                CheckPtr();
                return (_entry->lock_comment);
            }
        }
        /// <summary>
        /// Lock creation date or 0 if not locked in this WC
        /// </summary>
        public long LockCreationDate
        {
            get
            {
                CheckPtr();
                return (_entry->lock_creation_date);
            }
        }

        #endregion
    }
}
