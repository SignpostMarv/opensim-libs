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

namespace PumaCode.SvnDotNet.AprSharp {
    public struct AprString : IAprUnmanaged {
        private IntPtr mString;

        #region Generic embedding functions of an IntPtr
        public AprString(IntPtr ptr)
        {
            mString = ptr;
        }

        public AprString(byte[] bytes, AprPool pool)
        {
            mString = pool.Alloc(bytes.Length + 1);
            Marshal.Copy(bytes, 0, mString, bytes.Length);
            Marshal.WriteByte(mString, bytes.Length, 0);
        }

        public AprString(string str, AprPool pool)
        {
            mString = Apr.apr_pstrdup(pool, str);
        }

        public AprString(AprString str, AprPool pool)
        {
            mString = Apr.apr_pstrdup(pool, str);
        }

        public AprString(string str, int size, AprPool pool)
        {
            mString = Apr.apr_pstrndup(pool, str, unchecked((uint) size));
        }

        public AprString(AprString str, int size, AprPool pool)
        {
            mString = Apr.apr_pstrndup(pool, str, unchecked((uint) size));
        }

        [CLSCompliant(false)]
        public AprString(string str, uint size, AprPool pool)
        {
            mString = Apr.apr_pstrndup(pool, str, size);
        }

        [CLSCompliant(false)]
        public AprString(AprString str, uint size, AprPool pool)
        {
            mString = Apr.apr_pstrndup(pool, str, size);
        }

        public bool IsNull
        {
            get
            {
                return (mString == IntPtr.Zero);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            mString = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return mString;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(AprString str)
        {
            return str.mString;
        }

        public static implicit operator AprString(IntPtr ptr)
        {
            return new AprString(ptr);
        }

        public override string ToString()
        {
            if(IsNull)
                return ("[apr_string:NULL]");
            else
                return (Marshal.PtrToStringAnsi(mString));
        }

        public string Value
        {
            get { return IsNull ? null : Marshal.PtrToStringAnsi(mString); }
        }
        #endregion

        #region Methods wrappers
        public static AprString Duplicate(AprPool pool, string str)
        {
            return (new AprString(str, pool));
        }

        public static AprString Duplicate(AprPool pool, AprString str)
        {
            return (new AprString(str, pool));
        }

        public static AprString Duplicate(AprPool pool, string str, int size)
        {
            return (new AprString(str, size, pool));
        }

        [CLSCompliant(false)]
        public static AprString Duplicate(AprPool pool, string str, uint size)
        {
            return (new AprString(str, size, pool));
        }

        public static AprString Duplicate(AprPool pool, AprString str, int size)
        {
            return (new AprString(str, size, pool));
        }

        [CLSCompliant(false)]
        public static AprString Duplicate(AprPool pool, AprString str, uint size)
        {
            return (new AprString(str, size, pool));
        }

        public unsafe int Length
        {
            get
            {
                byte* p = (byte*) mString.ToPointer();
                while(*p++ != 0) ;
                return (unchecked((int) ((uint) p - (uint) mString.ToPointer() - 1)));
            }
        }

        #endregion
    }
}