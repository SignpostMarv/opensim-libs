using System;


namespace BulletDotNET
{
    public class btStridingMeshInterface
    {
        public static btStridingMeshInterface FromIntPtr(IntPtr handle)
        {
            return (btStridingMeshInterface)Native.GetObject(handle, typeof(btStridingMeshInterface));
        }

        public IntPtr m_handle = IntPtr.Zero;
        public bool m_displosed = false;

        public IntPtr Handle
        {
            get { return m_handle; }
        }

         // Don't use this one except by a base class.  You're responsible for setting m_handle!
        public btStridingMeshInterface()
        {

        }

        public btStridingMeshInterface(IntPtr handle)
        {
            m_handle = handle;
        }
        
    }
}
