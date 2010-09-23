using System;
using System.Collections.Generic;
using System.Text;

namespace Community.CsharpSqlite.src
{
    class AesManaged
    {
        public int KeySize;
        public System.Security.Cryptography.PaddingMode Padding;
        internal System.Security.Cryptography.ICryptoTransform CreateEncryptor(byte[] p, byte[] p_2)
        {
            throw new NotImplementedException();
        }

        internal System.Security.Cryptography.ICryptoTransform CreateDecryptor(byte[] p, byte[] p_2)
        {
            throw new NotImplementedException();
        }
    }
}
