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

namespace PumaCode.SvnDotNet.AprSharp {
    public unsafe struct AprMemNode {
        private apr_memnode_t* mMemNode;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct apr_memnode_t {
            public apr_memnode_t* next;
            public apr_memnode_t** selfref;
            public UInt32 index;
            public UInt32 free_index;
            public byte* first_avail;
            public byte* endp;
        }

        #region Generic embedding functions of an IntPtr
        private AprMemNode(apr_memnode_t* ptr)
        {
            mMemNode = ptr;
        }

        public AprMemNode(IntPtr ptr)
        {
            mMemNode = (apr_memnode_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (mMemNode == null);
            }
        }

        private void CheckPtr()
        {
            if(mMemNode == null)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            mMemNode = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(mMemNode);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(AprMemNode memNode)
        {
            return new IntPtr(memNode.mMemNode);
        }

        public static implicit operator AprMemNode(IntPtr ptr)
        {
            return new AprMemNode(ptr);
        }

        public override string ToString()
        {
            return ("[apr_memnode_t:" + (new IntPtr(mMemNode)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Structure members wrapper (Properties)

        public AprMemNode Next
        {
            get
            {
                CheckPtr();
                return (new AprMemNode(mMemNode->next));
            }
            set
            {
                CheckPtr();
                mMemNode->next = value.mMemNode;
            }
        }

        public AprMemNode Ref
        {
            get
            {
                CheckPtr();
                return ((mMemNode->selfref != null)
                        ? new AprMemNode(*(mMemNode->selfref))
                        : new AprMemNode(null));
            }
        }

        public IntPtr NativeRef
        {
            get
            {
                CheckPtr();
                return ((IntPtr) mMemNode->selfref);
            }
            set
            {
                CheckPtr();
                mMemNode->selfref = (apr_memnode_t**) value.ToPointer();
            }
        }

        public int Index
        {
            get
            {
                CheckPtr();
                return (unchecked((int) NativeIndex));
            }
        }

        [CLSCompliant(false)]
        public uint NativeIndex
        {
            get
            {
                CheckPtr();
                return (mMemNode->index);
            }
        }

        public int FreeIndex
        {
            get
            {
                CheckPtr();
                return (unchecked((int) NativeFreeIndex));
            }
        }

        [CLSCompliant(false)]
        public uint NativeFreeIndex
        {
            get
            {
                CheckPtr();
                return (mMemNode->free_index);
            }
        }

        public IntPtr FirstAvail
        {
            get
            {
                CheckPtr();
                return ((IntPtr) mMemNode->first_avail);
            }
            set
            {
                CheckPtr();
                mMemNode->first_avail = (byte*) value.ToPointer();
            }
        }

        [CLSCompliant(false)]
        public byte* NativeFirstAvail
        {
            get
            {
                CheckPtr();
                return (mMemNode->first_avail);
            }
            set
            {
                CheckPtr();
                mMemNode->first_avail = value;
            }
        }

        public IntPtr EndP
        {
            get
            {
                CheckPtr();
                return ((IntPtr) mMemNode->endp);
            }
        }

        [CLSCompliant(false)]
        public byte* NativeEndP
        {
            get
            {
                CheckPtr();
                return (mMemNode->endp);
            }
        }
        #endregion
    }
}