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
    public unsafe class SvnCommitInfo : IAprUnmanaged {
        private svn_commit_info_t* _commitInfo;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_commit_info_t {
            public int revision;
            public IntPtr date;
            public IntPtr author;
            public IntPtr post_commit_err;
        }

        #region Generic embedding functions of an IntPtr
        public SvnCommitInfo(IntPtr ptr)
        {
            _commitInfo = (svn_commit_info_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (_commitInfo == null);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            _commitInfo = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(_commitInfo);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnCommitInfo commitInfo)
        {
            return new IntPtr(commitInfo._commitInfo);
        }

        public static implicit operator SvnCommitInfo(IntPtr ptr)
        {
            return new SvnCommitInfo(ptr);
        }

        public override string ToString()
        {
            return ("[svn_commit_info_t:" + (new IntPtr(_commitInfo)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Properties wrappers
        public int Revision
        {
            get
            {
                CheckPtr();
                return (_commitInfo->revision);
            }
        }

        public AprString Date
        {
            get
            {
                CheckPtr();
                return (_commitInfo->date);
            }
        }

        public SvnData Author
        {
            get
            {
                CheckPtr();
                return (_commitInfo->author);
            }
        }

        public SvnData PostCommitErr
        {
            get
            {
                CheckPtr();
                return (_commitInfo->post_commit_err);
            }
        }
        #endregion
    }
}
