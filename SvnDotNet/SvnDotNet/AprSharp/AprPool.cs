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

    public struct AprPool : IAprUnmanaged {
        IntPtr mPool;

        #region Generic embedding functions of an IntPtr
        public AprPool(IntPtr ptr)
        {
            mPool = ptr;
        }

        public bool IsNull
        {
            get
            {
                return (mPool == IntPtr.Zero);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            mPool = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return mPool;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(AprPool pool)
        {
            return pool.mPool;
        }

        public static implicit operator AprPool(IntPtr ptr)
        {
            return new AprPool(ptr);
        }

        public override string ToString()
        {
            return ("[apr_pool_t:" + mPool.ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static AprPool Create()
        {
            return (Create(IntPtr.Zero, IntPtr.Zero));
        }

        public static AprPool Create(AprPool pool)
        {
            return (Create(pool, IntPtr.Zero));
        }

        public static AprPool Create(AprAllocator allocator)
        {
            return (Create(IntPtr.Zero, allocator));
        }

        public static AprPool Create(AprPool pool, AprAllocator allocator)
        {
            IntPtr ptr;

            Debug.Write(String.Format("apr_pool_create_ex({0},{1})...", pool, allocator));
            int res = Apr.apr_pool_create_ex(out ptr, pool,
                                             IntPtr.Zero, allocator);
            if(res != 0)
                throw new AprException(res);
            Debug.WriteLine(String.Format("Done({0:X})", ((Int32) ptr)));

            return (ptr);
        }

        public void Destroy()
        {
            CheckPtr();
            Debug.Write(String.Format("apr_pool_destroy({0})...", this));
            Apr.apr_pool_destroy(mPool);
            Debug.WriteLine("Done");
            ClearPtr();
        }

        public void Clear()
        {
            CheckPtr();
            Debug.Write(String.Format("apr_pool_clear({0})...", this));
            Apr.apr_pool_clear(mPool);
            Debug.WriteLine("Done");
        }

        public unsafe IntPtr Alloc(int size)
        {
            return ((IntPtr) Alloc(unchecked((uint) size)));
        }

        [CLSCompliant(false)]
        public unsafe byte* Alloc(uint size)
        {
            CheckPtr();
            Debug.WriteLine(String.Format("apr_palloc({0})", this));
            return ((byte*) Apr.apr_palloc(mPool, size));
        }

        public unsafe IntPtr CAlloc(int size)
        {
            return ((IntPtr) CAlloc(unchecked((uint) size)));
        }

        [CLSCompliant(false)]
        public unsafe byte* CAlloc(uint size)
        {
            CheckPtr();
            Debug.WriteLine(String.Format("apr_pcalloc({0})", this));
            return ((byte*) Apr.apr_pcalloc(mPool, size));
        }

        public bool IsAncestor(AprPool pool)
        {
            CheckPtr();
            Debug.WriteLine(String.Format("apr_pool_is_ancestor({0},{1})", this, pool));
            return (!(Apr.apr_pool_is_ancestor(mPool, pool) == 0));
        }
        #endregion

        #region Wrapper properties
        public AprAllocator Allocator
        {
            get
            {
                Debug.WriteLine(String.Format("apr_pool_allocator_get({0})", this));
                return (Apr.apr_pool_allocator_get(mPool));
            }
        }

        public AprPool Parent
        {
            get
            {
                Debug.WriteLine(String.Format("apr_pool_parent_get({0})", this));
                return (Apr.apr_pool_parent_get(mPool));
            }
        }
        #endregion

    }
}