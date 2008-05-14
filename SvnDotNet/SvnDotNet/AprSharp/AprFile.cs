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
using System.Text;
using System.Diagnostics;

namespace PumaCode.SvnDotNet.AprSharp {
    public struct AprFile : IAprUnmanaged {
        [Flags]
        public enum Flags {
            Read = 0x00001,
            Write = 0x00002,
            Create = 0x00004,
            Append = 0x00008,
            Truncate = 0x00010,
            Binary = 0x00020,
            Exclusive = 0x00040,
            Buffered = 0x00080,
            DelayOnClose = 0x00100,
            XThread = 0x00200,
            ShareLock = 0x00400,
            FileNoCleanUp = 0x00800,
            SendFileEnabled = 0x01000
        }

        [Flags]
        public enum Perms {
            SetUserId = 0x8000,
            UserRead = 0x0400,
            UserWrite = 0x0200,
            UserExecute = 0x0100,
            SetGroupId = 0x4000,
            GroupRead = 0x0040,
            GroupWrite = 0x0020,
            GroupExecute = 0x0010,
            WorldSticky = 0x2000,
            WorldRead = 0x0004,
            WorldWrite = 0x0002,
            WorldExecute = 0x0001,
            OSDefault = 0x0FFF,
            SourcePerms = 0x1000
        }

        public enum Std {
            In,
            Out,
            Err
        }

        IntPtr mFile;
        public string FileName;

        #region Generic embedding functions of an IntPtr
        public AprFile(IntPtr ptr)
        {
            mFile = ptr;
            FileName = null;
        }

        public bool IsNull
        {
            get
            {
                return (mFile == IntPtr.Zero);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            mFile = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return mFile;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(AprFile file)
        {
            return file.mFile;
        }

        public static implicit operator AprFile(IntPtr ptr)
        {
            return new AprFile(ptr);
        }

        public override string ToString()
        {
            return ("[apr_file_t:" + mFile.ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static AprFile Open(string fname, Flags flag, Perms perm, AprPool pool)
        {
            IntPtr ptr;

            Debug.Write(String.Format("apr_file_open({0},{1},{2},{3})...", fname, flag, perm, pool));
            int res = Apr.apr_file_open(out ptr, fname, (int) flag, (int) perm, pool);
            if(res != 0)
                throw new AprException(res);
            Debug.WriteLine(String.Format("Done({0:X})", ((Int32) ptr)));

            AprFile aprFile = (AprFile) ptr;
            aprFile.FileName = fname;

            return aprFile;
        }

        public static AprFile Open(Std stream, AprPool pool)
        {
            IntPtr ptr = IntPtr.Zero;
            int res = 0;

            switch(stream) {
                case Std.In:
                    Debug.Write(String.Format("apr_file_open_stdin({0})...", pool));
                    res = Apr.apr_file_open_stdin(out ptr, pool);
                    break;
                case Std.Out:
                    Debug.Write(String.Format("apr_file_open_stdout({0})...", pool));
                    res = Apr.apr_file_open_stdout(out ptr, pool);
                    break;
                case Std.Err:
                    Debug.Write(String.Format("apr_file_open_stderr({0})...", pool));
                    res = Apr.apr_file_open_stderr(out ptr, pool);
                    break;
            }
            if(res != 0)
                throw new AprException(res);
            Debug.WriteLine(String.Format("Done({0:X})", ((Int32) ptr)));

            return (ptr);
        }

        public void Close()
        {
            CheckPtr();
            Debug.Write(String.Format("apr_file_close({0})...", this));
            int res = Apr.apr_file_close(mFile);
            if(res != 0)
                throw new AprException(res);
            mFile = IntPtr.Zero;
        }
        #endregion
    }
}