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
using System.Runtime.InteropServices;
using System.Collections;

namespace PumaCode.SvnDotNet.AprSharp {
    public struct AprHash : IEnumerable, ICollection, IAprUnmanaged {
        IntPtr mHash;

        #region Generic embedding functions of an IntPtr
        public AprHash(IntPtr ptr)
        {
            mHash = ptr;
        }

        public bool IsNull
        {
            get
            {
                return (mHash == IntPtr.Zero);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            mHash = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return mHash;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(AprHash hashIndex)
        {
            return hashIndex.mHash;
        }

        public static implicit operator AprHash(IntPtr ptr)
        {
            return new AprHash(ptr);
        }

        public override string ToString()
        {
            return ("[apr_hash_t:" + mHash.ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Methods wrappers
        public static AprHash Make(AprPool pool)
        {
            IntPtr ptr;

            Debug.Write(String.Format("apr_hash_make({0})...", pool));
            ptr = Apr.apr_hash_make(pool);
            if(ptr == IntPtr.Zero)
                throw new AprException("apr_hash_make: Can't create an apr_hash_t");
            Debug.WriteLine(String.Format("Done({0:X})", ((Int32) ptr)));

            return (ptr);
        }

        public AprHash Copy(AprPool pool)
        {
            IntPtr ptr;

            CheckPtr();
            Debug.Write(String.Format("apr_hash_copy({0},{1})...", pool, this));
            ptr = Apr.apr_hash_copy(pool, mHash);
            if(ptr == IntPtr.Zero)
                throw new AprException("apr_hash_copy: Can't copy an apr_hash_t");
            Debug.WriteLine(String.Format("Done({0:X})", ((Int32) ptr)));

            return (ptr);
        }

        public AprHash Overlay(AprPool pool, AprHash h)
        {
            IntPtr ptr;

            CheckPtr();
            Debug.Write(String.Format("apr_hash_overlay({0},{1},{2})...", pool, h, this));
            ptr = Apr.apr_hash_overlay(pool, h, mHash);
            if(ptr == IntPtr.Zero)
                throw new AprException("apr_hash_overlay: Can't overlay an apr_hash_t");
            Debug.WriteLine(String.Format("Done({0:X})", ((Int32) ptr)));

            return (ptr);
        }

        public void Set(string key, string value)
        {
            AprString aprKey = new AprString(key, Pool);
            AprString aprValue = new AprString(value, Pool);
            Set((IntPtr) aprKey, -1, (IntPtr) aprValue);
        }

        public void Set(string key, IntPtr value)
        {
            AprString aprKey = new AprString(key, Pool);
            Set((IntPtr) aprKey, -1, value);
        }

        public void Set(AprString key, IntPtr value)
        {
            Set((IntPtr) key, -1, value);
        }

        public void Set(AprHashEntry item)
        {
            Set(item.Key, item.KeySize, item.Value);
        }

        public void Set(IntPtr key, int size, IntPtr value)
        {
            CheckPtr();
            Debug.WriteLine(String.Format("apr_hash_set({0},{1:X},{2},{3:X})", this, key.ToInt32(), size, value.ToInt32()));
            Apr.apr_hash_set(mHash, key, size, value);
        }

        public string GetAsString(string key)
        {
            AprString aprKey = new AprString(key, Pool);
            return (GetAsString((IntPtr) aprKey, -1));
        }

        public string GetAsString(AprString key)
        {
            return (GetAsString((IntPtr) key, -1));
        }

        public string GetAsString(IntPtr key, int size)
        {
            return (((AprString) Get(key, size)).ToString());
        }

        public IntPtr Get(string key)
        {
            AprString aprKey = new AprString(key, Pool);
            return (Get((IntPtr) aprKey, -1));
        }

        public IntPtr Get(AprString key)
        {
            return (Get((IntPtr) key, -1));
        }

        public IntPtr Get(AprHashEntry item)
        {
            return (Get(item.Key, item.KeySize));
        }

        public IntPtr Get(IntPtr key, int size)
        {
            IntPtr ptr;

            CheckPtr();
            Debug.Write(String.Format("apr_hash_get({0},{1:X},{2})...", this, key.ToInt32(), size));
            ptr = Apr.apr_hash_get(mHash, key, size);
            Debug.WriteLine(String.Format("Done({0:X})", ((Int32) ptr)));

            return (ptr);
        }

        public AprHashIndex First(AprPool pool)
        {
            return (AprHashIndex.First(pool, this));
        }
        #endregion

        #region Wrapper Properties
        public AprPool Pool
        {
            get
            {
                Debug.WriteLine(String.Format("apr_hash_pool_get({0:X})", this));
                return (Apr.apr_hash_pool_get(mHash));
            }
        }

        [CLSCompliant(false)]
        public uint NativeCount
        {
            get
            {
                Debug.WriteLine(String.Format("apr_hash_count({0:X})", this));
                return (Apr.apr_hash_count(mHash));
            }
        }
        #endregion

        #region ICollection
        public void CopyTo(Array array, int arrayIndex)
        {
            if(null == array)
                throw new AprArgumentNullException("array");
            if(arrayIndex < 0 || arrayIndex > array.Length)
                throw new AprArgumentOutOfRangeException("arrayIndex");
            if(array.Rank > 1)
                throw new AprArgumentException("array is multidimensional");
            if((array.Length - arrayIndex) < Count)
                throw new AprArgumentException("Not enough room from arrayIndex to end of array for this AprHash");

            int i = arrayIndex;
            IEnumerator it = GetEnumerator();
            while(it.MoveNext()) {
                array.SetValue(it.Current, i++);
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public int Count
        {
            get
            {
                Debug.WriteLine(String.Format("apr_hash_count({0:X})", this));
                return (unchecked((int) Apr.apr_hash_count(mHash)));
            }
        }
        #endregion

        #region IEnumerable
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator) new AprHashEnumerator(this, Pool);
        }
        #endregion

        public IEnumerator GetEnumerator(AprPool pool)
        {
            return (IEnumerator) new AprHashEnumerator(this, pool);
        }
    }

    // AprHashItem Class
    public struct AprHashEntry {
        private AprPool mPool;
        internal IntPtr mKey;
        internal int mKeySize;
        internal IntPtr mValue;

        public AprHashEntry(AprPool pool)
        {
            mPool = pool;
            mKey = IntPtr.Zero;
            mKeySize = 0;
            mValue = IntPtr.Zero;
        }

        public AprPool Pool
        {
            get
            {
                return mPool;
            }
            set
            {
                mPool = value;
            }
        }

        public IntPtr Key
        {
            get
            {
                return mKey;
            }
            set
            {
                mKey = value;
            }
        }

        public int KeySize
        {
            get
            {
                return mKeySize;
            }
            set
            {
                mKeySize = value;
            }
        }

        public IntPtr Value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
            }
        }

        public string KeyAsString
        {
            get
            {
                return Marshal.PtrToStringAnsi(mKey);
            }
            set
            {
                if(mPool.IsNull)
                    throw new AprNullReferenceException();
                mKey = new AprString(value, mPool);
                mKeySize = -1;
            }
        }

        public string ValueAsString
        {
            get
            {
                return Marshal.PtrToStringAnsi(mValue);
            }
            set
            {
                if(mPool.IsNull)
                    throw new AprNullReferenceException();
                mValue = new AprString(value, mPool);
            }
        }
    }

    // AprHashEnumerator Class
    public class AprHashEnumerator : IEnumerator {
        private AprPool mPool;
        private AprHash mHash;
        private AprHashIndex mHashIndex;
        private bool reset;

        public AprHashEnumerator(AprHash h)
        {
            mHash = h;
            mPool = h.Pool;
            reset = true;
        }

        public AprHashEnumerator(AprHash h, AprPool pool)
        {
            mHash = h;
            mPool = pool;
            reset = true;
        }

        #region IEnumerator
        public bool MoveNext()
        {
            if(reset) {
                mHashIndex = AprHashIndex.First(mPool, mHash);
                reset = false;
            }
            else if(!mHashIndex.IsNull) {
                mHashIndex.Next();
            }
            return (!mHashIndex.IsNull);
        }

        public void Reset()
        {
            mHashIndex.ClearPtr();
            reset = true;
        }

        public object Current
        {
            get
            {
                AprHashEntry entry = new AprHashEntry(mPool);

                mHashIndex.This(out entry.mKey, out entry.mKeySize, out entry.mValue);
                return (entry);
            }
        }
        #endregion
    }
}