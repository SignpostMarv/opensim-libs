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
    public unsafe class SvnDirEnt : IAprUnmanaged {
        private svn_dirent_t* _dirEnt;

#if WIN32
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
#else
		[StructLayout( LayoutKind.Sequential, Pack=4 )]
#endif
        private struct svn_dirent_t {
            public int kind;
            public long size;
            public int has_props;
            public int created_rev;
            public long time;
            public IntPtr last_author;
        }

        #region Generic embedding functions of an IntPtr
        public SvnDirEnt(IntPtr ptr)
        {
            _dirEnt = (svn_dirent_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (_dirEnt == null);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            _dirEnt = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(_dirEnt);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnDirEnt entry)
        {
            return new IntPtr(entry._dirEnt);
        }

        public static implicit operator SvnDirEnt(IntPtr ptr)
        {
            return new SvnDirEnt(ptr);
        }

        public override string ToString()
        {
            return ("[svn_dirent_t:" + (new IntPtr(_dirEnt)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Properties wrappers
        public Svn.NodeKind Kind
        {
            get
            {
                CheckPtr();
                return ((Svn.NodeKind) _dirEnt->kind);
            }
        }

        public long Size
        {
            get
            {
                CheckPtr();
                return (_dirEnt->size);
            }
        }

        public bool HasProps
        {
            get
            {
                CheckPtr();
                return (_dirEnt->has_props != 0);
            }
        }

        public int CreationRevision
        {
            get
            {
                CheckPtr();
                return (_dirEnt->created_rev);
            }
        }

        public long Time
        {
            get
            {
                CheckPtr();
                return (_dirEnt->time);
            }
        }

        public SvnData LastAuthor
        {
            get
            {
                CheckPtr();
                return (_dirEnt->last_author);
            }
        }
        #endregion
    }
}
