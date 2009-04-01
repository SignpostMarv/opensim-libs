using System;

namespace BulletDotNET
{
    public static class Native
    {
        public const string Dll = "libbulletnet";

        public static object GetObject(IntPtr raw, Type t)
        {
            if (raw == IntPtr.Zero)
                throw new InvalidOperationException();
            object o;

            try
            {
                o = Activator.CreateInstance(t, raw);
            } 
            catch (TypeLoadException)
            {
                throw new InvalidOperationException();
            }

            return o;
        }
    }
}
