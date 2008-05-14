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
using System.Text;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.SubversionSharp {
    public struct SvnData : IAprUnmanaged {
        private IntPtr _string;

        internal static UTF8Encoding Encoder = new UTF8Encoding();

        #region Generic embedding functions of an IntPtr
        public SvnData(IntPtr ptr)
        {
            _string = ptr;
        }

        public SvnData(string str, AprPool pool)
        {
            byte[] utf8str = Encoder.GetBytes(str);
            _string = pool.Alloc(utf8str.Length + 1);
            Marshal.Copy(utf8str, 0, _string, utf8str.Length);
            Marshal.WriteByte(_string, utf8str.Length, 0);
        }

        public SvnData(AprString str, AprPool pool)
        {
            SvnError err = Svn.svn_utf_cstring_to_utf8(out _string, str, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
        }

        public SvnData(SvnString str, AprPool pool)
        {
            IntPtr svnStr;
            SvnError err = Svn.svn_utf_string_to_utf8(out svnStr, str, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
            _string = ((SvnString) svnStr).Data;
        }

        public SvnData(SvnStringBuf str, AprPool pool)
        {
            IntPtr svnStrBuf;
            SvnError err = Svn.svn_utf_stringbuf_to_utf8(out svnStrBuf, str, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
            _string = ((SvnStringBuf) svnStrBuf).Data;
        }

        public bool IsNull
        {
            get
            {
                return (_string == IntPtr.Zero);
            }
        }

        public void ClearPtr()
        {
            _string = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return _string;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnData str)
        {
            return str._string;
        }

        public static implicit operator SvnData(IntPtr ptr)
        {
            return new SvnData(ptr);
        }

        public override string ToString()
        {
            if(IsNull)
                return ("[svn_data:NULL]");
            else {
                int len = new AprString(_string).Length;
                if(len == 0)
                    return ("");
                byte[] str = new byte[len];
                Marshal.Copy(_string, str, 0, len);
                return (Encoder.GetString(str));
            }
        }
        #endregion

        #region Methods wrappers
        public static SvnData Duplicate(AprPool pool, string str)
        {
            return (new SvnData(str, pool));
        }

        public static SvnData Duplicate(AprPool pool, AprString str)
        {
            return (new SvnData(str, pool));
        }

        public static SvnData Duplicate(AprPool pool, SvnString str)
        {
            return (new SvnData(str, pool));
        }

        public static SvnData Duplicate(AprPool pool, SvnStringBuf str)
        {
            return (new SvnData(str, pool));
        }
        #endregion
    }
}