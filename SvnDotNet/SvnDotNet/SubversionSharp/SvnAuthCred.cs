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
    public unsafe struct SvnAuthCredSimple : IAprUnmanaged {
        private svn_auth_cred_simple_t* _credentials;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_auth_cred_simple_t {
            public IntPtr username;
            public IntPtr password;
            public int may_save;
        }

        #region Generic embedding functions of an IntPtr
        private SvnAuthCredSimple(svn_auth_cred_simple_t* ptr)
        {
            _credentials = ptr;
        }

        public SvnAuthCredSimple(IntPtr ptr)
        {
            _credentials = (svn_auth_cred_simple_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (_credentials == null);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            _credentials = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(_credentials);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnAuthCredSimple cred)
        {
            return new IntPtr(cred._credentials);
        }

        public static implicit operator SvnAuthCredSimple(IntPtr ptr)
        {
            return new SvnAuthCredSimple(ptr);
        }

        public override string ToString()
        {
            return ("[svn_auth_cred_simple_t:" + (new IntPtr(_credentials)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static SvnAuthCredSimple Alloc(AprPool pool)
        {
            return (new SvnAuthCredSimple((svn_auth_cred_simple_t*)
                       pool.CAlloc(sizeof(svn_auth_cred_simple_t))));
        }
        #endregion

        #region Wrapper Properties
        public AprString Username
        {
            get
            {
                CheckPtr();
                return (new AprString(_credentials->username));
            }
            set
            {
                CheckPtr();
                _credentials->username = value;
            }
        }

        public AprString Password
        {
            get
            {
                CheckPtr();
                return (_credentials->password);
            }
            set
            {
                CheckPtr();
                _credentials->password = value;
            }
        }

        public bool MaySave
        {
            get
            {
                CheckPtr();
                return (_credentials->may_save != 0);
            }
            set
            {
                CheckPtr();
                _credentials->may_save = (value) ? 1 : 0;
            }
        }
        #endregion
    }

    public unsafe struct SvnAuthCredUsername {
        private svn_auth_cred_username_t* mCred;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_auth_cred_username_t {
            public IntPtr username;
            public int may_save;
        }

        #region Generic embedding functions of an IntPtr
        private SvnAuthCredUsername(svn_auth_cred_username_t* ptr)
        {
            mCred = ptr;
        }

        public SvnAuthCredUsername(IntPtr ptr)
        {
            mCred = (svn_auth_cred_username_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (mCred == null);
            }
        }

        public void ClearPtr()
        {
            mCred = null;
        }

        public static implicit operator IntPtr(SvnAuthCredUsername cred)
        {
            return new IntPtr(cred.mCred);
        }

        public static implicit operator SvnAuthCredUsername(IntPtr ptr)
        {
            return new SvnAuthCredUsername(ptr);
        }

        public override string ToString()
        {
            return ("[svn_auth_cred_username_t:" + (new IntPtr(mCred)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static SvnAuthCredUsername Alloc(AprPool pool)
        {
            return (new SvnAuthCredUsername((svn_auth_cred_username_t*)
                         pool.CAlloc(sizeof(svn_auth_cred_username_t))));
        }
        #endregion

        #region Wrapper Properties
        public AprString Username
        {
            get
            {
                return (mCred->username);
            }
            set
            {
                mCred->username = value;
            }
        }

        public bool MaySave
        {
            get
            {
                return (mCred->may_save != 0);
            }
            set
            {
                mCred->may_save = (value) ? 1 : 0;
            }
        }
        #endregion
    }

    public unsafe struct SvnAuthCredSslServerTrust {
        [Flags]
        public enum CertFailures {
            NotYetValid = 0x00000001,
            Expired = 0x00000002,
            CNMismatch = 0x00000004,
            UnknownCA = 0x00000008,
            Other = 0x40000000
        }

        private svn_auth_cred_ssl_server_trust_t* mCred;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_auth_cred_ssl_server_trust_t {
            public int may_save;
            public uint accepted_failures;
        }

        #region Generic embedding functions of an IntPtr
        private SvnAuthCredSslServerTrust(svn_auth_cred_ssl_server_trust_t* ptr)
        {
            mCred = ptr;
        }

        public SvnAuthCredSslServerTrust(IntPtr ptr)
        {
            mCred = (svn_auth_cred_ssl_server_trust_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (mCred == null);
            }
        }

        public void ClearPtr()
        {
            mCred = null;
        }

        public static implicit operator IntPtr(SvnAuthCredSslServerTrust cred)
        {
            return new IntPtr(cred.mCred);
        }

        public static implicit operator SvnAuthCredSslServerTrust(IntPtr ptr)
        {
            return new SvnAuthCredSslServerTrust(ptr);
        }

        public override string ToString()
        {
            return ("[svn_auth_cred_ssl_server_trust_t:" + (new IntPtr(mCred)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static SvnAuthCredSslServerTrust Alloc(AprPool pool)
        {
            return (new SvnAuthCredSslServerTrust((svn_auth_cred_ssl_server_trust_t*)
                               pool.CAlloc(sizeof(svn_auth_cred_ssl_server_trust_t))));
        }
        #endregion

        #region Wrapper Properties
        public bool MaySave
        {
            get
            {
                return (mCred->may_save != 0);
            }
            set
            {
                mCred->may_save = (value) ? 1 : 0;
            }
        }

        public CertFailures AcceptedFailures
        {
            get
            {
                return ((CertFailures) mCred->accepted_failures);
            }
            set
            {
                mCred->accepted_failures = (uint) value;
            }
        }
        #endregion
    }

    public unsafe struct SvnAuthCredSslClientCert {
        private svn_auth_cred_ssl_client_cert_t* mCred;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_auth_cred_ssl_client_cert_t {
            public IntPtr cert_file;
            public int may_save;
        }

        #region Generic embedding functions of an IntPtr
        private SvnAuthCredSslClientCert(svn_auth_cred_ssl_client_cert_t* ptr)
        {
            mCred = ptr;
        }

        public SvnAuthCredSslClientCert(IntPtr ptr)
        {
            mCred = (svn_auth_cred_ssl_client_cert_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (mCred == null);
            }
        }

        public void ClearPtr()
        {
            mCred = null;
        }

        public static implicit operator IntPtr(SvnAuthCredSslClientCert cred)
        {
            return new IntPtr(cred.mCred);
        }

        public static implicit operator SvnAuthCredSslClientCert(IntPtr ptr)
        {
            return new SvnAuthCredSslClientCert(ptr);
        }

        public override string ToString()
        {
            return ("[svn_auth_cred_ssl_client_cert_t:" + (new IntPtr(mCred)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static SvnAuthCredSslClientCert Alloc(AprPool pool)
        {
            return (new SvnAuthCredSslClientCert((svn_auth_cred_ssl_client_cert_t*)
                              pool.CAlloc(sizeof(svn_auth_cred_ssl_client_cert_t))));
        }
        #endregion

        #region Wrapper Properties
        public SvnPath CertFile
        {
            get
            {
                return (mCred->cert_file);
            }
            set
            {
                mCred->cert_file = value;
            }
        }

        public bool MaySave
        {
            get
            {
                return (mCred->may_save != 0);
            }
            set
            {
                mCred->may_save = (value) ? 1 : 0;
            }
        }
        #endregion
    }

    public unsafe struct SvnAuthCredSslClientCertPw {
        private svn_auth_cred_ssl_client_cert_pw_t* mCred;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_auth_cred_ssl_client_cert_pw_t {
            public IntPtr cert_file;
            public int may_save;
        }

        #region Generic embedding functions of an IntPtr
        private SvnAuthCredSslClientCertPw(svn_auth_cred_ssl_client_cert_pw_t* ptr)
        {
            mCred = ptr;
        }

        public SvnAuthCredSslClientCertPw(IntPtr ptr)
        {
            mCred = (svn_auth_cred_ssl_client_cert_pw_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (mCred == null);
            }
        }

        public void ClearPtr()
        {
            mCred = null;
        }

        public static implicit operator IntPtr(SvnAuthCredSslClientCertPw cred)
        {
            return new IntPtr(cred.mCred);
        }

        public static implicit operator SvnAuthCredSslClientCertPw(IntPtr ptr)
        {
            return new SvnAuthCredSslClientCertPw(ptr);
        }

        public override string ToString()
        {
            return ("[svn_auth_cred_ssl_client_cert_pw_t:" + (new IntPtr(mCred)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static SvnAuthCredSslClientCertPw Alloc(AprPool pool)
        {
            return (new SvnAuthCredSslClientCertPw((svn_auth_cred_ssl_client_cert_pw_t*)
                                pool.CAlloc(sizeof(svn_auth_cred_ssl_client_cert_pw_t))));
        }
        #endregion

        #region Wrapper Properties
        public SvnPath CertFile
        {
            get
            {
                return (mCred->cert_file);
            }
            set
            {
                mCred->cert_file = value;
            }
        }

        public bool MaySave
        {
            get
            {
                return (mCred->may_save != 0);
            }
            set
            {
                mCred->may_save = (value) ? 1 : 0;
            }
        }
        #endregion
    }
}