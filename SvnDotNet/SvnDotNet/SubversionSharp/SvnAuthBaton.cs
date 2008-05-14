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
using System.Collections;
using System.Runtime.InteropServices;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.SubversionSharp {
    public struct SvnAuthBaton : IAprUnmanaged {
        public enum Param {
            DefaultUsername,
            DefaultPassword,
            NonInteractive,
            NoAuthCache,
            SslServerFailures,
            SslServerCertInfo,
            Config,
            ServerGroup,
            ConfigDir
        };

        private static readonly string[] ParamName = new string[] { 
			"svn:auth:username",
			"svn:auth:password",
			"svn:auth:non-interactive",
			"svn:auth:no-auth-cache",
			"svn:auth:ssl:failures",
			"svn:auth:ssl:cert-info",
			"svn:auth:config",
			"svn:auth:server-group",
			"svn:auth:config-dir"
		};

        private IntPtr _authBaton;
        private IntPtr[] _paramName;
        private IntPtr _pool;
        private readonly ArrayList _authProviders;

        #region Generic embedding functions of an IntPtr
        private SvnAuthBaton(IntPtr ptr)
        {
            _authBaton = ptr;
            _authProviders = null;
            _paramName = null;
            _pool = IntPtr.Zero;
        }

        public AprPool Pool
        {
            get
            {
                return (_pool);
            }
            set
            {
                _pool = value;
            }
        }

        private SvnAuthBaton(ArrayList authProviders, AprPool pool)
        {
            AprArray authArray = AprArray.Make(pool, authProviders.Count, Marshal.SizeOf(typeof(IntPtr)));
            _authProviders = new ArrayList();
            foreach(SvnAuthProviderObject authObj in authProviders) {
                Marshal.WriteIntPtr(authArray.Push(), authObj);
                _authProviders.Add(authObj.AuthProvider);
            }
            Debug.Write(String.Format("svn_auth_open({0},{1})...", authArray, pool));
            Svn.svn_auth_open(out _authBaton, authArray, pool);
            Debug.WriteLine(String.Format("Done({0:X})", _authBaton.ToInt32()));
            _paramName = null;
            _pool = pool;
        }

        public bool IsNull
        {
            get
            {
                return (_authBaton == IntPtr.Zero || _pool == IntPtr.Zero);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            _authBaton = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return _authBaton;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnAuthBaton authBaton)
        {
            return authBaton._authBaton;
        }

        public static implicit operator SvnAuthBaton(IntPtr ptr)
        {
            return new SvnAuthBaton(ptr);
        }

        public override string ToString()
        {
            return ("[svn_auth_baton_t:" + _authBaton.ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static SvnAuthBaton Open(ArrayList authProviders, AprPool pool)
        {
            return (new SvnAuthBaton(authProviders, pool));
        }

        public void SetParameter(Param param, IntPtr value)
        {
            CheckPtr();
            if(_paramName == null) {
                _paramName = new IntPtr[ParamName.Length];
                for(int i = 0; i < ParamName.Length; i++)
                    _paramName[i] = IntPtr.Zero;
            }

            if(_paramName[(int) param] == IntPtr.Zero)
                _paramName[(int) param] = new AprString(ParamName[(int) param], _pool);

            Debug.WriteLine(String.Format("svn_auth_set_parameter({0},{1:X},{2:X})", this, _paramName[(int) param].ToInt32(), value.ToInt32()));
            Svn.svn_auth_set_parameter(_authBaton, _paramName[(int) param], value);
        }

        public IntPtr GetParameter(Param param)
        {
            CheckPtr();
            if(_paramName == null) {
                _paramName = new IntPtr[ParamName.Length];
                for(int i = 0; i < ParamName.Length; i++)
                    _paramName[i] = IntPtr.Zero;
            }

            if(_paramName[(int) param] == IntPtr.Zero)
                _paramName[(int) param] = new AprString(ParamName[(int) param], _pool);

            IntPtr ptr;
            Debug.Write(String.Format("svn_auth_get_parameter({0},{1:X})...", this, _paramName[(int) param].ToInt32()));
            ptr = Svn.svn_auth_get_parameter(_authBaton, _paramName[(int) param]);
            Debug.WriteLine(String.Format("Done({0:X})", ptr.ToInt32()));
            return (ptr);
        }
        #endregion
    }
}