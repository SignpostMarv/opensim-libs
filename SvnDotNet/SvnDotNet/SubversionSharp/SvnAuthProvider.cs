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

namespace PumaCode.SvnDotNet.SubversionSharp {
    public struct SvnAuthProviderObject : IAprUnmanaged {
        private IntPtr _authProviderObject;
        private readonly SvnDelegate _authProvider;

        #region Generic embedding functions of an IntPtr
        private SvnAuthProviderObject(IntPtr ptr)
        {
            _authProviderObject = ptr;
            _authProvider = null;
        }

        private SvnAuthProviderObject(IntPtr ptr, SvnDelegate authProvider)
        {
            _authProviderObject = ptr;
            _authProvider = authProvider;
        }

        public bool IsNull
        {
            get
            {
                return (_authProviderObject == IntPtr.Zero);
            }
        }

        public void ClearPtr()
        {
            _authProviderObject = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return _authProviderObject;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnAuthProviderObject authProviderObject)
        {
            return authProviderObject._authProviderObject;
        }

        public static implicit operator SvnAuthProviderObject(IntPtr ptr)
        {
            return new SvnAuthProviderObject(ptr);
        }

        public override string ToString()
        {
            return ("[svn_auth_provider_object_t:" + _authProviderObject.ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper delegates
        public delegate SvnError SimplePrompt(out SvnAuthCredSimple cred, IntPtr baton,
                                              AprString realm, AprString username,
                                              bool maySave, AprPool pool);

        public delegate SvnError UsernamePrompt(out SvnAuthCredUsername cred, IntPtr baton,
                                                AprString realm, bool maySave,
                                                AprPool pool);

        public delegate SvnError SslServerTrustPrompt(out SvnAuthCredSslServerTrust cred,
                                                      IntPtr baton, AprString realm,
                                                      SvnAuthCredSslServerTrust.CertFailures failures,
                                                      SvnAuthSslServerCertInfo certInfo,
                                                      bool maySave, IntPtr pool);

        public delegate SvnError SslClientCertPrompt(out SvnAuthCredSslClientCert cred,
                                                     IntPtr baton, AprString realm,
                                                     bool maySave, IntPtr pool);

        public delegate SvnError SslClientCertPwPrompt(out SvnAuthCredSslClientCertPw cred,
                                                       IntPtr baton, AprString realm,
                                                       bool maySave, IntPtr pool);
        #endregion

        #region Wrapper client method
        public static SvnAuthProviderObject GetPromptProvider(
                                                    SimplePrompt promptFunc,
                                                    IntPtr promptBaton, int retryLimit, AprPool pool)
        {
            IntPtr authObj;
            SvnDelegate auth = new SvnDelegate(promptFunc);
            Debug.Write(String.Format("svn_client_get_simple_prompt_provider([callback:{0}],{1:X},{2},{3})...", promptFunc.Method.Name, promptBaton.ToInt32(), retryLimit, pool));
            Svn.svn_client_get_simple_prompt_provider(out authObj,
                                            (Svn.svn_auth_simple_prompt_func_t) auth.Wrapper,
                                            promptBaton, retryLimit, pool);
            Debug.WriteLine(String.Format("Done({0:X})", authObj.ToInt32()));
            return (new SvnAuthProviderObject(authObj, auth));
        }

        public static SvnAuthProviderObject GetPromptProvider(
                                                UsernamePrompt promptFunc,
                                                IntPtr promptBaton, int retryLimit, AprPool pool)
        {
            IntPtr authObj;
            SvnDelegate auth = new SvnDelegate(promptFunc);
            Debug.Write(String.Format("svn_client_get_username_prompt_provider([callback:{0}],{1:X},{2},{3})...", promptFunc.Method.Name, promptBaton.ToInt32(), retryLimit, pool));
            Svn.svn_client_get_username_prompt_provider(out authObj,
                                            (Svn.svn_auth_username_prompt_func_t) auth.Wrapper,
                                            promptBaton, retryLimit, pool);
            Debug.WriteLine(String.Format("Done({0:X})", authObj.ToInt32()));
            return (new SvnAuthProviderObject(authObj, auth));
        }

        public static SvnAuthProviderObject GetSimpleProvider(AprPool pool)
        {
            IntPtr authObj;
            Debug.Write(String.Format("svn_client_get_simple_provider({0})", pool));
            Svn.svn_client_get_simple_provider(out authObj, pool);
            Debug.WriteLine(String.Format("Done({0:X})", authObj.ToInt32()));
            return (new SvnAuthProviderObject(authObj));
        }

        public static SvnAuthProviderObject GetUsernameProvider(AprPool pool)
        {
            IntPtr authObj;
            Debug.Write(String.Format("svn_client_get_username_provider({0})", pool));
            Svn.svn_client_get_username_provider(out authObj, pool);
            Debug.WriteLine(String.Format("Done({0:X})", authObj.ToInt32()));
            return (new SvnAuthProviderObject(authObj));
        }

        public static SvnAuthProviderObject GetSslServerTrustFileProvider(AprPool pool)
        {
            IntPtr authObj;
            Debug.Write(String.Format("svn_client_get_ssl_server_trust_file_provider({0})", pool));
            Svn.svn_client_get_ssl_server_trust_file_provider(out authObj, pool);
            Debug.WriteLine(String.Format("Done({0:X})", authObj.ToInt32()));
            return (new SvnAuthProviderObject(authObj));
        }

        public static SvnAuthProviderObject GetSslClientCertFileProvider(AprPool pool)
        {
            IntPtr authObj;
            Debug.Write(String.Format("svn_client_get_ssl_client_cert_file_provider({0})", pool));
            Svn.svn_client_get_ssl_client_cert_file_provider(out authObj, pool);
            Debug.WriteLine(String.Format("Done({0:X})", authObj.ToInt32()));
            return (new SvnAuthProviderObject(authObj));
        }

        public static SvnAuthProviderObject GetSslClientCertPwFileProvider(AprPool pool)
        {
            IntPtr authObj;
            Debug.Write(String.Format("svn_client_get_ssl_client_cert_pw_file_provider({0})", pool));
            Svn.svn_client_get_ssl_client_cert_pw_file_provider(out authObj, pool);
            Debug.WriteLine(String.Format("Done({0:X})", authObj.ToInt32()));
            return (new SvnAuthProviderObject(authObj));
        }

        public static SvnAuthProviderObject GetPromptProvider(
                                                SslServerTrustPrompt promptFunc,
                                                IntPtr promptBaton, AprPool pool)
        {
            IntPtr authObj;
            SvnDelegate auth = new SvnDelegate(promptFunc);
            Debug.Write(String.Format("svn_client_get_ssl_server_trust_prompt_provider([callback:{0}],{1:X},{2})...", promptFunc.Method.Name, promptBaton.ToInt32(), pool));
            Svn.svn_client_get_ssl_server_trust_prompt_provider(out authObj,
                                        (Svn.svn_auth_ssl_server_trust_prompt_func_t) auth.Wrapper,
                                        promptBaton, pool);
            Debug.WriteLine(String.Format("Done({0:X})", authObj.ToInt32()));
            return (new SvnAuthProviderObject(authObj, auth));
        }

        public static SvnAuthProviderObject GetPromptProvider(
                                                SslClientCertPrompt promptFunc,
                                                IntPtr promptBaton, int retryLimit, AprPool pool)
        {
            IntPtr authObj;
            SvnDelegate auth = new SvnDelegate(promptFunc);
            Debug.Write(String.Format("svn_client_get_ssl_client_cert_prompt_provider([callback:{0}],{1},{2},{3})...", auth.Wrapper, promptBaton, retryLimit, pool));
            Svn.svn_client_get_ssl_client_cert_prompt_provider(out authObj,
                                        (Svn.svn_auth_ssl_client_cert_prompt_func_t) auth.Wrapper,
                                        promptBaton, retryLimit, pool);
            Debug.WriteLine(String.Format("Done({0:X})", authObj.ToInt32()));
            return (new SvnAuthProviderObject(authObj, auth));
        }

        public static SvnAuthProviderObject GetPromptProvider(
                                                SslClientCertPwPrompt promptFunc,
                                                IntPtr promptBaton, int retryLimit, AprPool pool)
        {
            IntPtr authObj;
            SvnDelegate auth = new SvnDelegate(promptFunc);
            Debug.Write(String.Format("svn_client_get_ssl_client_cert_pw_prompt_provider([callback:{0}],{1:X},{2},{3})...", promptFunc.Method.Name, promptBaton.ToInt32(), retryLimit, pool));
            Svn.svn_client_get_ssl_client_cert_pw_prompt_provider(out authObj,
                                        (Svn.svn_auth_ssl_client_cert_pw_prompt_func_t) auth.Wrapper,
                                        promptBaton, retryLimit, pool);
            Debug.WriteLine(String.Format("Done({0:X})", authObj.ToInt32()));
            return (new SvnAuthProviderObject(authObj, auth));
        }
        #endregion

        public SvnDelegate AuthProvider
        {
            get { return _authProvider; }
        }

    }
}