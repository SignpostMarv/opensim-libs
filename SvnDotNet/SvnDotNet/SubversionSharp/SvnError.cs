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
    public unsafe class SvnError : IAprUnmanaged {
        private svn_error_t* _error;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_error_t {
            public int apr_err;
            public IntPtr message;
            public svn_error_t* child;
            public IntPtr pool;
            public IntPtr file;
            public int line;
        }

        #region Generic embedding functions of an IntPtr
        private SvnError(svn_error_t* ptr)
        {
            _error = ptr;
        }

        public SvnError(IntPtr ptr)
        {
            _error = (svn_error_t*) ptr.ToPointer();
        }

        public static SvnError NoError = new SvnError(IntPtr.Zero);

        public bool IsNoError
        {
            get
            {
                return (_error == null);
            }
        }

        public bool IsNull
        {
            get
            {
                return (_error == null);
            }
        }

        private void CheckPtr()
        {
            if(_error == null)
                throw new SvnNullReferenceException();
        }

        public void ClearPtr()
        {
            _error = null;
        }

        public IntPtr ToIntPtr()
        {
            if(IsNoError)
                return IntPtr.Zero;
            else
                return new IntPtr(_error);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnError error)
        {
            if(error.IsNoError)
                return IntPtr.Zero;
            else
                return new IntPtr(error._error);
        }

        public static implicit operator SvnError(IntPtr ptr)
        {
            return new SvnError(ptr);
        }

        public override string ToString()
        {
            return ("[svn_error_t:" + (new IntPtr(_error)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static SvnError Create(int aprErr, SvnError err, string str)
        {
            return (new SvnError(Svn.svn_error_create(aprErr, new IntPtr(err._error), str)));
        }

        public void Clear()
        {
            CheckPtr();
            Svn.svn_error_clear(new IntPtr(_error));
        }
        #endregion

        #region Wrapper properties
        public int AprErr
        {
            get
            {
                CheckPtr();
                return (_error->apr_err);
            }
        }

        public AprString Message
        {
            get
            {
                CheckPtr();
                return (_error->message);
            }
        }

        public SvnError Child
        {
            get
            {
                CheckPtr();
                return (new SvnError(_error->child));
            }
        }

        public AprPool Pool
        {
            get
            {
                CheckPtr();
                return (_error->pool);
            }
        }

        public AprString File
        {
            get
            {
                CheckPtr();
                return (_error->file);
            }
        }

        public int Line
        {
            get
            {
                CheckPtr();
                return (_error->line);
            }
        }
        #endregion
    }
}