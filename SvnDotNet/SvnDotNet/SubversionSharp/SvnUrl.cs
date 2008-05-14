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
    public struct SvnUrl : IAprUnmanaged {
        private IntPtr _url;

        internal static UTF8Encoding Encoder = new UTF8Encoding();

        #region Check Url
        public static bool IsUrl(string pathOrUrl)
        {
            return SvnPath.PathIsUrl(pathOrUrl);
        }
        #endregion

        #region Generic embedding functions of an IntPtr
        public SvnUrl(IntPtr ptr)
        {
            _url = ptr;
        }

        public SvnUrl(string str, AprPool pool)
        {
            if(!IsUrl(str)) {
                throw new SvnException("Invalid URL: " + str);
            }
            _url = Svn.svn_path_uri_encode(new SvnPath(str, pool), pool);
        }

        [Obsolete("Uri class can't handle all svn URL styles")]
        public SvnUrl(Uri uri, AprPool pool)
        {
            _url = new AprString(uri.AbsoluteUri, pool);
        }

        public SvnUrl(AprString str, AprPool pool)
        {
            _url = Svn.svn_path_uri_encode(new SvnPath(str, pool), pool);
        }

        public SvnUrl(SvnString str, AprPool pool)
        {
            _url = Svn.svn_path_uri_encode(new SvnPath(str, pool), pool);
        }

        public SvnUrl(SvnStringBuf str, AprPool pool)
        {
            _url = Svn.svn_path_uri_encode(new SvnPath(str, pool), pool);
        }

        public bool IsNull
        {
            get
            {
                return (_url == IntPtr.Zero);
            }
        }

        public void ClearPtr()
        {
            _url = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return _url;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnUrl url)
        {
            return url._url;
        }

        public static implicit operator SvnUrl(IntPtr ptr)
        {
            return new SvnUrl(ptr);
        }

        /// <summary>
        /// Converts the URL to a string.
        /// </summary>
        /// <returns>The URL as a string, or "[svn_url:NULL]" if null.</returns>
        public override string ToString()
        {
            return IsNull ? "[svn_url:NULL]" : GetInternalString();
        }

        /// <summary>
        /// Returns the URL as a string, or null.
        /// </summary>
        public string Value
        {
            get { return IsNull ? null : GetInternalString(); }
        }

        private string GetInternalString()
        {
            if(IsNull)
                throw new InvalidOperationException("Can't call GetInternalString when Value is null");

            AprString urlString = new AprString(_url);
            int len = urlString.Length;
            if(len == 0)
                return ("");

            byte[] str = new byte[len];
            Marshal.Copy(_url, str, 0, len);

            // System.Uri.UnescapeDataString is not yet implemented in Mono 2.0.
            try {
                return (Uri.UnescapeDataString(Encoder.GetString(str)));
            }
            catch(NotImplementedException) {
                return (Encoder.GetString(str));
            }
        }
        #endregion

        #region Methods wrappers
        public static SvnUrl Duplicate(AprPool pool, string str)
        {
            return (new SvnUrl(str, pool));
        }

        public static SvnUrl Duplicate(AprPool pool, AprString str)
        {
            return (new SvnUrl(str, pool));
        }

        public static SvnUrl Duplicate(AprPool pool, SvnString str)
        {
            return (new SvnUrl(str, pool));
        }

        public static SvnUrl Duplicate(AprPool pool, SvnStringBuf str)
        {
            return (new SvnUrl(str, pool));
        }
        #endregion
    }
}