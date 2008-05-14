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
    public struct SvnPath : IAprUnmanaged {
        private IntPtr _path;

        internal static UTF8Encoding Encoder = new UTF8Encoding();

        #region Generic embedding functions of an IntPtr
        public SvnPath(IntPtr ptr)
        {
            _path = ptr;
        }

        public SvnPath(string str, AprPool pool)
        {
            byte[] utf8str = Encoder.GetBytes(PathInternalStyle(str));
            _path = pool.Alloc(utf8str.Length + 1);
            Marshal.Copy(utf8str, 0, _path, utf8str.Length);
            Marshal.WriteByte(_path, utf8str.Length, 0);
        }

        public SvnPath(AprString str, AprPool pool)
        {
            SvnError err = Svn.svn_utf_cstring_to_utf8(out _path, str, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
        }

        public SvnPath(SvnString str, AprPool pool)
        {
            IntPtr svnStr;
            SvnError err = Svn.svn_utf_string_to_utf8(out svnStr, str, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
            _path = ((SvnString) svnStr).Data;
        }

        public SvnPath(SvnStringBuf str, AprPool pool)
        {
            IntPtr svnStrBuf;
            SvnError err = Svn.svn_utf_stringbuf_to_utf8(out svnStrBuf, str, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
            _path = ((SvnStringBuf) svnStrBuf).Data;
        }

        public bool IsNull
        {
            get
            {
                return (_path == IntPtr.Zero);
            }
        }

        public void ClearPtr()
        {
            _path = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return _path;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnPath str)
        {
            return str._path;
        }

        public static implicit operator SvnPath(IntPtr ptr)
        {
            return new SvnPath(ptr);
        }

        /// <summary>
        /// Converts the path to a string.
        /// </summary>
        /// <returns>The path as a string, or "[svn_path:NULL]" if null.</returns>
        public override string ToString()
        {
            return IsNull ? "[svn_path:NULL]" : GetInternalString();
        }

        /// <summary>
        /// Returns the path as a string, or null.
        /// </summary>
        public string Value
        {
            get { return IsNull ? null : GetInternalString(); }
        }

        private string GetInternalString()
        {
            if(IsNull)
                throw new InvalidOperationException("Can't call GetInternalString when Value is null");

            int len = new AprString(_path).Length;
            if(len == 0)
                return ("");
            byte[] str = new byte[len];
            Marshal.Copy(_path, str, 0, len);
            return PathLocalStyle(Encoder.GetString(str));
        }

        #endregion

        #region Methods wrappers
        public static SvnPath Duplicate(AprPool pool, string str)
        {
            return (new SvnPath(str, pool));
        }

        public static SvnPath Duplicate(AprPool pool, AprString str)
        {
            return (new SvnPath(str, pool));
        }

        public static SvnPath Duplicate(AprPool pool, SvnString str)
        {
            return (new SvnPath(str, pool));
        }

        public static SvnPath Duplicate(AprPool pool, SvnStringBuf str)
        {
            return (new SvnPath(str, pool));
        }
        #endregion
        /// <summary>
        /// Path looks like a valid URL?
        /// Replacement for svn_path_is_url function.
        /// </summary>
        /// <param name="path">path to check</param>
        /// <returns>If path looks like if valid URL returns true, otherwise false</returns>
        public static bool PathIsUrl(string path)
        {
            return (PathInternalStyle(path).IndexOf("://") > 0);
        }

        #region Path translation
        /// <summary>
        /// Translate path to internal representation.
        /// </summary>
        /// <param name="path">path to translate</param>
        /// <returns>translated path</returns>
        public static string PathInternalStyle(string path)
        {
            if(System.IO.Path.DirectorySeparatorChar != '/') {
                return path.Replace(System.IO.Path.DirectorySeparatorChar, '/');
            }
            else {
                return path;
            }
        }

        /// <summary>
        /// Translate path to local representation (OS depended)
        /// </summary>
        /// <param name="path">path to translate</param>
        /// <returns>translated path</returns>
        public static string PathLocalStyle(string path)
        {
            if(System.IO.Path.DirectorySeparatorChar != '/') {
                return path.Replace('/', System.IO.Path.DirectorySeparatorChar);
            }
            else {
                return path;
            }
        }
        #endregion

    }
}