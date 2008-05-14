using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.SubversionSharp {
    /// <summary>
    /// Summary description for SvnIO.
    /// </summary>
    public class SvnIO {
        public static string OpenUniqueFile(out AprFile file, string path, string suffix, bool deleteOnClose, AprPool pool)
        {
            IntPtr newFile;
            IntPtr uniqueFileName;
            AprString utf8path = new AprString(System.Text.Encoding.UTF8.GetBytes(path), pool);
            AprString utf8suffix = new AprString(System.Text.Encoding.UTF8.GetBytes(suffix), pool);
            SvnError error = Svn.svn_io_open_unique_file(out newFile, out uniqueFileName, utf8path, utf8suffix, deleteOnClose ? 1 : 0, pool);
            if(!error.IsNoError)
                throw new SvnException(error);
            file = new AprFile(newFile);

            AprString utf8uniqueFileName = new AprString(uniqueFileName);
            byte[] bytes = new byte[utf8uniqueFileName.Length];
            Marshal.Copy(utf8uniqueFileName.ToIntPtr(), bytes, 0, bytes.Length);

            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
