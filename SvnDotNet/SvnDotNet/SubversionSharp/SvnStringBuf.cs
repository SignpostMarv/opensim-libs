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
    public unsafe class SvnStringBuf : IAprUnmanaged {
        private svn_stringbuf_t* _stringBuf;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_stringbuf_t {
            public IntPtr pool;
            public IntPtr data;
            public uint len;
            public uint blocksize;
        }

        #region Generic embedding functions of an IntPtr
        public SvnStringBuf(IntPtr ptr)
        {
            _stringBuf = (svn_stringbuf_t*) ptr.ToPointer();
        }

        public SvnStringBuf(AprString str, AprPool pool)
        {
            _stringBuf = (svn_stringbuf_t*) (Svn.svn_stringbuf_create(str, pool)).ToPointer();
        }

        public SvnStringBuf(string str, AprPool pool)
        {
            _stringBuf = (svn_stringbuf_t*) (Svn.svn_stringbuf_create(str, pool)).ToPointer();
        }

        public SvnStringBuf(SvnStringBuf str, AprPool pool)
        {
            _stringBuf = (svn_stringbuf_t*) (Svn.svn_stringbuf_dup(str, pool)).ToPointer();
        }

        public SvnStringBuf(SvnString str, AprPool pool)
        {
            _stringBuf = (svn_stringbuf_t*) (Svn.svn_stringbuf_create_from_string(str, pool)).ToPointer();
        }

        public SvnStringBuf(AprString str, int size, AprPool pool)
        {
            _stringBuf = (svn_stringbuf_t*) (Svn.svn_stringbuf_ncreate(str, unchecked((uint) size),
                                                               pool)).ToPointer();
        }

        public SvnStringBuf(string str, int size, AprPool pool)
        {
            _stringBuf = (svn_stringbuf_t*) (Svn.svn_stringbuf_ncreate(str, unchecked((uint) size),
                                                               pool)).ToPointer();
        }

        [CLSCompliant(false)]
        public SvnStringBuf(AprString str, uint size, AprPool pool)
        {
            _stringBuf = (svn_stringbuf_t*) (Svn.svn_stringbuf_ncreate(str, size, pool)).ToPointer();
        }

        [CLSCompliant(false)]
        public SvnStringBuf(string str, uint size, AprPool pool)
        {
            _stringBuf = (svn_stringbuf_t*) (Svn.svn_stringbuf_ncreate(str, size, pool)).ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (_stringBuf == null);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            _stringBuf = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(_stringBuf);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnStringBuf str)
        {
            return new IntPtr(str._stringBuf);
        }

        public static implicit operator SvnStringBuf(IntPtr ptr)
        {
            return new SvnStringBuf(ptr);
        }

        public override string ToString()
        {
            if(IsNull)
                return ("[svn_stringbuf:NULL]");
            else
                return (Marshal.PtrToStringAnsi(_stringBuf->data));
        }
        #endregion

        #region Method wrappers
        public static SvnStringBuf Create(AprString str, AprPool pool)
        {
            return (new SvnStringBuf(str, pool));
        }

        public static SvnStringBuf Create(string str, AprPool pool)
        {
            return (new SvnStringBuf(str, pool));
        }

        public static SvnStringBuf Create(SvnStringBuf str, AprPool pool)
        {
            return (new SvnStringBuf(str, pool));
        }

        public static SvnStringBuf Create(SvnString str, AprPool pool)
        {
            return (new SvnStringBuf(str, pool));
        }

        public static SvnStringBuf Create(AprString str, int size, AprPool pool)
        {
            return (new SvnStringBuf(str, size, pool));
        }

        public static SvnStringBuf Create(string str, int size, AprPool pool)
        {
            return (new SvnStringBuf(str, size, pool));
        }

        [CLSCompliant(false)]
        public static SvnStringBuf Create(AprString str, uint size, AprPool pool)
        {
            return (new SvnStringBuf(str, size, pool));
        }

        [CLSCompliant(false)]
        public static SvnStringBuf Create(string str, uint size, AprPool pool)
        {
            return (new SvnStringBuf(str, size, pool));
        }
        #endregion

        #region Properties wrappers
        public AprString Data
        {
            get
            {
                CheckPtr();
                return (new AprString(_stringBuf->data));
            }
        }

        public int Len
        {
            get
            {
                CheckPtr();
                return (unchecked((int) _stringBuf->len));
            }
        }

        [CLSCompliant(false)]
        public uint NativeLen
        {
            get
            {
                CheckPtr();
                return (_stringBuf->len);
            }
        }
        #endregion
    }
}
