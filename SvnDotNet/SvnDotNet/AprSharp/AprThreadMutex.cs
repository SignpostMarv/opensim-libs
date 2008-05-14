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

namespace PumaCode.SvnDotNet.AprSharp {
    public struct AprThreadMutex : IAprUnmanaged {
        IntPtr mThreadMutex;

        ///<remark>Should be synchronized with #define from APR</remarks>
        public enum AprThreadMutexFlags {
            Default,
            Nested,
            Unnested
        }

        #region Generic embedding functions of an IntPtr
        public AprThreadMutex(IntPtr ptr)
        {
            mThreadMutex = ptr;
        }

        public bool IsNull
        {
            get
            {
                return (mThreadMutex == IntPtr.Zero);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            mThreadMutex = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return mThreadMutex;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(AprThreadMutex threadMutex)
        {
            return threadMutex.mThreadMutex;
        }

        public static implicit operator AprThreadMutex(IntPtr ptr)
        {
            return new AprThreadMutex(ptr);
        }

        public override string ToString()
        {
            return ("[apr_thread_mutex_t:" + mThreadMutex.ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static AprThreadMutex Create(AprPool pool)
        {
            return (Create(AprThreadMutexFlags.Default, pool));
        }

        public static AprThreadMutex Create(AprThreadMutexFlags flags,
                                             AprPool pool)
        {
            IntPtr ptr;

            Debug.Write(String.Format("apr_thread_mutex_create({0},{1})...", flags, pool));
            int res = Apr.apr_thread_mutex_create(out ptr, (uint) flags, pool);
            if(res != 0)
                throw new AprException(res);
            Debug.WriteLine(String.Format("Done({0:X})", ((Int32) ptr)));

            return ((AprThreadMutex) ptr);
        }

        public void Destroy()
        {
            CheckPtr();
            Debug.Write(String.Format("apr_thread_mutex_destroy({0:X})...", ((Int32) mThreadMutex)));
            int res = Apr.apr_thread_mutex_destroy(mThreadMutex);
            if(res != 0)
                throw new AprException(res);
            Debug.WriteLine("Done");
            ClearPtr();
        }

        public void Lock()
        {
            CheckPtr();
            Debug.Write(String.Format("apr_thread_mutex_lock({0:X})...", ((Int32) mThreadMutex)));
            int res = Apr.apr_thread_mutex_lock(mThreadMutex);
            if(res != 0)
                throw new AprException(res);
            Debug.WriteLine("Done");
        }

        public bool TryLock()
        {
            CheckPtr();
            Debug.Write(String.Format("apr_thread_mutex_trylock({0:X})...", ((Int32) mThreadMutex)));
            int res = Apr.apr_thread_mutex_trylock(mThreadMutex);
            if(res != 0) {
                if(res == 70025) {
                    Debug.WriteLine(String.Format("Fail({0})", res));
                    return (false);
                }
                throw new AprException(res);
            }
            Debug.WriteLine(String.Format("Done({0})", res));
            return (true);
        }

        public void Unlock()
        {
            CheckPtr();
            Debug.Write(String.Format("apr_thread_mutex_unlock({0:X})...", ((Int32) mThreadMutex)));
            int res = Apr.apr_thread_mutex_unlock(mThreadMutex);
            if(res != 0)
                throw new AprException(res);
            Debug.WriteLine("Done");
        }
        #endregion

        #region Wrapper properties
        public AprPool Pool
        {
            get
            {
                Debug.WriteLine(String.Format("apr_thread_mutex_pool_get({0:X})", ((Int32) mThreadMutex)));
                return (Apr.apr_thread_mutex_pool_get(mThreadMutex));
            }
        }
        #endregion
    }
}