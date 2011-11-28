using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace BulletDotNET
{
    public class ContactAddedCallbackHandler
    {
        protected IntPtr m_handle;
        protected bool m_disposed;

        public struct ContactInfo
        {
            public uint contact;
            public uint contactWith;
            public float pX;    // contact position
            public float pY;
            public float pZ;
            public float nX;    // contact normal
            public float nY;
            public float nZ;
            public float depth; // contact depth
        };

        uint[] m_collisionsPinned;
        GCHandle m_collisionsPinnedHandle;

        public IntPtr Handle
        {
            get { return m_handle; }
        }

        protected static ContactAddedCallbackHandler FromIntPtr(IntPtr handle)
        {
            return (ContactAddedCallbackHandler)Native.GetObject(handle,
                typeof(ContactAddedCallbackHandler));
        }

        public ContactAddedCallbackHandler()
        {

        }
        public ContactAddedCallbackHandler(btDiscreteDynamicsWorld world)
        {
            // create a block of pinned memory that is used to pass collision info back
            // mem[0] = size of the pinned block
            // mem[1] = returned number of collisions IDs
            // mem[2] = collision ID
            // ...
            m_collisionsPinned = new uint[10000];
            m_collisionsPinnedHandle = GCHandle.Alloc(m_collisionsPinned, GCHandleType.Pinned);
            m_handle = BulletHelper_CreateContactCollector(world.Handle, m_collisionsPinnedHandle.AddrOfPinnedObject());
            System.Console.WriteLine("After create {0}", m_handle);
            m_collisionsPinned[0] = 10000;  // zero is total number of IntPtrs
            m_collisionsPinned[1] = 0;      // one is the number of entries returned
            m_collisionsPinned[2] = 9;      // stride of the contact information
        }

        public List<ContactInfo> GetContactList()
        {
            List<ContactInfo> contacts = new List<ContactInfo>();
            // System.Console.WriteLine("Before FetchContact {0}", m_handle);
            BulletHelper_FetchContact(m_handle);
            uint numEntries = m_collisionsPinned[1];
            uint offsetStride = m_collisionsPinned[2];
            uint offset = 3;     // size of header (max, cnt, stride)
            for (int ii = 0; ii < numEntries ; ii++)
            {
                ContactInfo ci = new ContactInfo();
                ci.contact = m_collisionsPinned[offset + 0];
                ci.contactWith = m_collisionsPinned[offset + 1];
                ci.pX = m_collisionsPinned[offset + 2] / 1000.0f;
                ci.pY = m_collisionsPinned[offset + 3] / 1000.0f;
                ci.pZ = m_collisionsPinned[offset + 4] / 1000.0f;
                ci.nX = m_collisionsPinned[offset + 5] / 1000.0f;
                ci.nY = m_collisionsPinned[offset + 6] / 1000.0f;
                ci.nZ = m_collisionsPinned[offset + 7] / 1000.0f;
                ci.depth = m_collisionsPinned[offset + 8] / 1000.0f;
                offset += offsetStride;
                // if collision info is zero, don't pass the info up
                // if (ci.X == 0 && ci.Y == 0 && ci.Z == 0) continue;
                contacts.Add(ci);
            }

            return contacts;
        }

        public void Clear()
        {
            BulletHelper_ClearContacts(m_handle);
        }

        #region Native Invokes
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletHelper_CreateContactCollector(IntPtr world, IntPtr mem);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletHelper_FetchContact(IntPtr cch);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern int BulletHelper_FetchDebugValue();
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern IntPtr BulletHelper_ClearContacts(IntPtr cch);
         [DllImport(Native.Dll), SuppressUnmanagedCodeSecurity]
        static extern void BulletHelper_DestroyContactCollector();
        #endregion Native Invokes

        public void Dispose()
        {
        }
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
