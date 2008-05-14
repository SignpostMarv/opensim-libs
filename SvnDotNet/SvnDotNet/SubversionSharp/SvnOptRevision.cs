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
    public class SvnRevision {
        Svn.Revision _kind;
        int _number;
        long _date;

        #region Ctors
        public SvnRevision(Svn.Revision kind)
        {
            _kind = kind;
            _number = 0;
            _date = 0;
        }

        public SvnRevision(int number)
        {
            _kind = Svn.Revision.Number;
            _number = number;
            _date = 0;
        }

        public SvnRevision(long date)
        {
            _kind = Svn.Revision.Date;
            _number = 0;
            _date = date;
        }

        internal SvnRevision(SvnOptRevision rev)
        {
            _kind = rev.Kind;
            if(_kind == Svn.Revision.Number)
                _number = rev.Number;
            else if(_kind == Svn.Revision.Date)
                _date = rev.Date;
        }
        #endregion

        #region Operators
        public static implicit operator SvnRevision(Svn.Revision revision)
        {
            return new SvnRevision(revision);
        }

        public static implicit operator SvnRevision(int revision)
        {
            return new SvnRevision(revision);
        }

        public static implicit operator SvnRevision(long revision)
        {
            return new SvnRevision(revision);
        }

        //        public static implicit operator SvnRevision(SvnOptRevision revision)
        //       {
        //           return new SvnRevision(revision);
        //       }
        #endregion

        #region Methods
        internal SvnOptRevision ToSvnOpt(AprPool pool)
        {
            return (new SvnOptRevision(this, pool));
        }

        internal SvnOptRevision ToSvnOpt(out GCHandle handle)
        {
            return (new SvnOptRevision(this, out handle));
        }
        #endregion

        #region Properties
        public Svn.Revision Kind
        {
            get
            {
                return (_kind);
            }
            set
            {
                _kind = value;
            }
        }

        public int Number
        {
            get
            {
                if(_kind != Svn.Revision.Number)
                    throw new AprNullReferenceException();
                return (_number);
            }
            set
            {
                _kind = Svn.Revision.Number;
                _number = value;
            }
        }

        public long Date
        {
            get
            {
                if(_kind != Svn.Revision.Date)
                    throw new AprNullReferenceException();
                return (_date);
            }
            set
            {
                _kind = Svn.Revision.Date;
                _date = value;
            }
        }

        internal SvnOptRevision Revision
        {
            set
            {
                _kind = value.Kind;
                if(_kind == Svn.Revision.Number)
                    _number = value.Number;
                else if(_kind == Svn.Revision.Date)
                    _date = value.Date;
            }
        }
        #endregion
    }

    internal unsafe struct SvnOptRevision : IAprUnmanaged {
        private svn_opt_revision_t* _optRevision;

        [StructLayout(LayoutKind.Explicit)]
        private struct svn_opt_revision_t {
            [FieldOffset(0)]
            public int kind;
#if WIN32
            [FieldOffset(8)]
            public int number;
            [FieldOffset(8)]
            public long date;
#else
			[FieldOffset(4)]public int number;
     		[FieldOffset(4)]public long date;
#endif
        }

        #region Generic embedding functions of an IntPtr
        public SvnOptRevision(IntPtr ptr)
        {
            _optRevision = (svn_opt_revision_t*) ptr.ToPointer();
        }

        public SvnOptRevision(AprPool pool)
        {
            _optRevision = (svn_opt_revision_t*) pool.CAlloc(sizeof(svn_opt_revision_t));
        }

        public SvnOptRevision(Svn.Revision revKind, AprPool pool)
        {
            _optRevision = (svn_opt_revision_t*) pool.CAlloc(sizeof(svn_opt_revision_t));
            Kind = revKind;
        }

        public SvnOptRevision(int revNum, AprPool pool)
        {
            _optRevision = (svn_opt_revision_t*) pool.CAlloc(sizeof(svn_opt_revision_t));
            Number = revNum;
        }

        public SvnOptRevision(long revDate, AprPool pool)
        {
            _optRevision = (svn_opt_revision_t*) pool.CAlloc(sizeof(svn_opt_revision_t));
            Date = revDate;
        }

        public SvnOptRevision(SvnRevision rev, AprPool pool)
        {
            _optRevision = (svn_opt_revision_t*) pool.CAlloc(sizeof(svn_opt_revision_t));
            Revision = rev;
        }

        public SvnOptRevision(out GCHandle handle)
        {
            handle = GCHandle.Alloc(new svn_opt_revision_t(), GCHandleType.Pinned);
            _optRevision = (svn_opt_revision_t*) handle.AddrOfPinnedObject().ToPointer();
        }

        public SvnOptRevision(Svn.Revision revKind, out GCHandle handle)
        {
            handle = GCHandle.Alloc(new svn_opt_revision_t(), GCHandleType.Pinned);
            _optRevision = (svn_opt_revision_t*) handle.AddrOfPinnedObject().ToPointer();
            Kind = revKind;
        }

        public SvnOptRevision(int revNum, out GCHandle handle)
        {
            handle = GCHandle.Alloc(new svn_opt_revision_t(), GCHandleType.Pinned);
            _optRevision = (svn_opt_revision_t*) handle.AddrOfPinnedObject().ToPointer();
            Number = revNum;
        }

        public SvnOptRevision(long revDate, out GCHandle handle)
        {
            handle = GCHandle.Alloc(new svn_opt_revision_t(), GCHandleType.Pinned);
            _optRevision = (svn_opt_revision_t*) handle.AddrOfPinnedObject().ToPointer();
            Date = revDate;
        }

        public SvnOptRevision(SvnRevision rev, out GCHandle handle)
        {
            handle = GCHandle.Alloc(new svn_opt_revision_t(), GCHandleType.Pinned);
            _optRevision = (svn_opt_revision_t*) handle.AddrOfPinnedObject().ToPointer();
            Revision = rev;
        }

        public bool IsNull
        {
            get
            {
                return (_optRevision == null);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            _optRevision = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(_optRevision);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnOptRevision revision)
        {
            return new IntPtr(revision._optRevision);
        }

        public static implicit operator SvnOptRevision(IntPtr ptr)
        {
            return new SvnOptRevision(ptr);
        }

        public override string ToString()
        {
            return ("[svn_opt_revision_t:" + (new IntPtr(_optRevision)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Properties wrappers
        public Svn.Revision Kind
        {
            get
            {
                CheckPtr();
                return ((Svn.Revision) _optRevision->kind);
            }
            set
            {
                CheckPtr();
                _optRevision->kind = (int) value;
            }
        }

        public int Number
        {
            get
            {
                CheckPtr();
                if((Svn.Revision) _optRevision->kind != Svn.Revision.Number)
                    throw new AprNullReferenceException();
                return (_optRevision->number);
            }
            set
            {
                CheckPtr();
                _optRevision->kind = Revision.Number;
                _optRevision->number = value;
            }
        }

        public long Date
        {
            get
            {
                CheckPtr();
                if((Svn.Revision) _optRevision->kind != Svn.Revision.Date)
                    throw new AprNullReferenceException();
                return (_optRevision->date);
            }
            set
            {
                CheckPtr();
                _optRevision->kind = (int) Revision.Date;
                _optRevision->date = value;
            }
        }

        public SvnRevision Revision
        {
            get
            {
                CheckPtr();
                return (new SvnRevision(this));
            }
            set
            {
                CheckPtr();
                _optRevision->kind = (int) value.Kind;
                if((Svn.Revision) _optRevision->kind == Svn.Revision.Number)
                    _optRevision->number = value.Number;
                else if((Svn.Revision) _optRevision->kind == Svn.Revision.Date)
                    _optRevision->date = value.Date;
            }
        }
        #endregion
    }
}
