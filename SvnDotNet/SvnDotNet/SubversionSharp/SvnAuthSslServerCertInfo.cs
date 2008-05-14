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
using PumaCode.SvnDotNet.AprSharp;
using System.Runtime.InteropServices;

namespace PumaCode.SvnDotNet.SubversionSharp {

    public unsafe struct SvnAuthSslServerCertInfo : IAprUnmanaged {
        private svn_auth_ssl_server_cert_info* _sslServerCertInfo;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_auth_ssl_server_cert_info {
            public IntPtr hostname;
            public IntPtr fingerprint;
            public IntPtr valid_from;
            public IntPtr valid_until;
            public IntPtr issuer_dname;
            public IntPtr ascii_cert;
        }

        #region Generic embedding functions of an IntPtr
        public SvnAuthSslServerCertInfo(IntPtr ptr)
        {
            _sslServerCertInfo = (svn_auth_ssl_server_cert_info*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (_sslServerCertInfo == null);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            _sslServerCertInfo = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(_sslServerCertInfo);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnAuthSslServerCertInfo clientContext)
        {
            return new IntPtr(clientContext._sslServerCertInfo);
        }

        public static implicit operator SvnAuthSslServerCertInfo(IntPtr ptr)
        {
            return new SvnAuthSslServerCertInfo(ptr);
        }

        public override string ToString()
        {
            return ("[svn_client_context_t:" + (new IntPtr(_sslServerCertInfo)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper Properties
        public AprString Hostname
        {
            get
            {
                CheckPtr();
                return (_sslServerCertInfo->hostname);
            }
        }
        public AprString Fingerprint
        {
            get
            {
                CheckPtr();
                return (_sslServerCertInfo->fingerprint);
            }
        }
        public AprString ValidFrom
        {
            get
            {
                CheckPtr();
                return (_sslServerCertInfo->valid_from);
            }
        }
        public AprString ValidUntil
        {
            get
            {
                CheckPtr();
                return (_sslServerCertInfo->valid_until);
            }
        }
        public AprString IssuerDName
        {
            get
            {
                CheckPtr();
                return (_sslServerCertInfo->issuer_dname);
            }
        }
        public AprString AsciiCert
        {
            get
            {
                CheckPtr();
                return (_sslServerCertInfo->ascii_cert);
            }
        }
        #endregion
    }
}