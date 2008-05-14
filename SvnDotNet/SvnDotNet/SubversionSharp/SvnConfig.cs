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

    public struct SvnConfig : IAprUnmanaged {
        IntPtr _config;

        #region Generic embedding functions of an IntPtr
        private SvnConfig(IntPtr ptr)
        {
            _config = ptr;
        }

        public bool IsNull
        {
            get
            {
                return (_config == IntPtr.Zero);
            }
        }

        public void ClearPtr()
        {
            _config = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return _config;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnConfig config)
        {
            return config._config;
        }

        public static implicit operator SvnConfig(IntPtr ptr)
        {
            return new SvnConfig(ptr);
        }

        public override string ToString()
        {
            return ("[svn_config_t:" + _config.ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static void Ensure()
        {
            AprPool pool = Svn.PoolCreate();
            try {
                Ensure(pool);
            }
            finally {
                pool.Destroy();
            }
        }

        public static void Ensure(AprPool pool)
        {
            Debug.WriteLine(String.Format("svn_config_ensure(NULL,{0})", pool));
            SvnError err = Svn.svn_config_ensure(IntPtr.Zero, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
        }

        public static void Ensure(SvnPath configDir, AprPool pool)
        {
            Debug.WriteLine(String.Format("svn_config_ensure({0},{1})", configDir, pool));
            SvnError err = Svn.svn_config_ensure(configDir, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
        }

        public static AprHash GetConfig(AprPool pool)
        {
            IntPtr h;
            Debug.WriteLine(String.Format("svn_config_get_config(NULL,{0})", pool));
            SvnError err = Svn.svn_config_get_config(out h, IntPtr.Zero, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
            return h;
        }

        public static AprHash GetConfig(SvnPath configDir, AprPool pool)
        {
            IntPtr h;
            Debug.WriteLine(String.Format("svn_config_get_config({0},{1})", configDir, pool));
            SvnError err = Svn.svn_config_get_config(out h, configDir, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
            return h;
        }
        #endregion
    }
}