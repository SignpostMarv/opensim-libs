//  AprSharp, a wrapper library around the Apache Portable Runtime Library
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
//		http://www.softec.st/AprSharp
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
using System.Collections;

namespace PumaCode.SvnDotNet.AprSharp {
    public struct AprHashIndex : IAprUnmanaged {
        IntPtr mHashIndex;

        #region Generic embedding functions of an IntPtr
        public AprHashIndex(IntPtr ptr)
        {
            mHashIndex = ptr;
        }

        public bool IsNull
        {
            get
            {
                return (mHashIndex == IntPtr.Zero);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            mHashIndex = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return mHashIndex;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(AprHashIndex hashIndex)
        {
            return hashIndex.mHashIndex;
        }

        public static implicit operator AprHashIndex(IntPtr ptr)
        {
            return new AprHashIndex(ptr);
        }

        public override string ToString()
        {
            return ("[apr_hash_index_t:" + mHashIndex.ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Methods wrappers
        public static AprHashIndex First(AprPool pool, AprHash h)
        {
            IntPtr ptr;

            Debug.Write(String.Format("apr_hash_first({0},{1})...", pool, h));
            ptr = Apr.apr_hash_first(pool, h);
            Debug.WriteLine(String.Format("Done({0:X})", ((Int32) ptr)));

            return (ptr);
        }

        public void Next()
        {
            IntPtr ptr;

            CheckPtr();
            Debug.Write(String.Format("apr_hash_next({0})...", this));
            ptr = Apr.apr_hash_next(mHashIndex);
            Debug.WriteLine(String.Format("Done({0:X})", ((Int32) ptr)));
            mHashIndex = ptr;
        }

        public void This(out string key, out string value)
        {
            IntPtr v;

            This(out key, out v);
            value = Marshal.PtrToStringAnsi(v);
        }

        public void This(out string key, out IntPtr value)
        {
            IntPtr k;
            int size;

            This(out k, out size, out value);
            key = Marshal.PtrToStringAnsi(k);
        }

        public void This(out IntPtr key, out int size, out IntPtr value)
        {
            CheckPtr();
            Debug.Write(String.Format("apr_hash_this({0})...", this));
            Apr.apr_hash_this(mHashIndex, out key, out size, out value);
            Debug.WriteLine(String.Format("Done({0:X},{1},{2:X})", key.ToInt32(), size, value.ToInt32()));
        }
        #endregion
    }
}