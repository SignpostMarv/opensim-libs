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
    public unsafe class SvnLogChangedPath : IAprUnmanaged {
        private svn_log_changed_path_t* _entry;

#if WIN32
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
#else
		[StructLayout( LayoutKind.Sequential, Pack=4 )]
#endif
        private struct svn_log_changed_path_t {
            /** 'A'dd, 'D'elete, 'R'eplace, 'M'odify */
            public byte action;
            /** Source path of copy (if any). */
            public IntPtr copyfrom_path;
            /** Source revision of copy (if any). */
            public int copyfrom_rev;
        }

        #region Generic embedding functions of an IntPtr
        public SvnLogChangedPath(IntPtr ptr)
        {
            _entry = (svn_log_changed_path_t*) ptr.ToPointer();
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

        public static implicit operator IntPtr(SvnLogChangedPath entry)
        {
            return new IntPtr(entry._entry);
        }

        public static implicit operator SvnLogChangedPath(IntPtr ptr)
        {
            return new SvnLogChangedPath(ptr);
        }

        public override string ToString()
        {
            return ("[svn_log_changed_path_t:" + (new IntPtr(_entry)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Properties wrappers
        public char Action
        {
            get
            {
                CheckPtr();
                return ((char) _entry->action);
            }
        }

        public int CopyFromRev
        {
            get
            {
                CheckPtr();
                return (_entry->copyfrom_rev);
            }
        }

        public AprString CopyFromPath
        {
            get
            {
                CheckPtr();
                return (_entry->copyfrom_path);
            }
        }
        #endregion
    }
}
