//  SubversionSharp, a wrapper library around the Subversion client API
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
//		http://www.softec.st/SubversionSharp
//		Support@softec.st
//
//  Initial authors : 
//		Denis Gervalle
//		Olivier Desaive
#endregion
//
using System;
using System.Diagnostics;
using PumaCode.SvnDotNet.AprSharp;

namespace PumaCode.SvnDotNet.SubversionSharp {
    public struct SvnStream : IAprUnmanaged {
        public delegate SvnError ReadFunc(IntPtr baton, IntPtr buffer, ref int len);
        public delegate SvnError WriteFunc(IntPtr baton, IntPtr data, ref int len);
        public delegate SvnError CloseFunc(IntPtr baton);

        private IntPtr _stream;
        private SvnDelegate _readDelegate;
        private SvnDelegate _writeDelegate;
        private SvnDelegate _closeDelegate;

        #region Generic embedding functions of an IntPtr
        private SvnStream(IntPtr ptr)
        {
            _stream = ptr;
            _readDelegate = null;
            _writeDelegate = null;
            _closeDelegate = null;
        }

        public SvnStream(IntPtr baton, AprPool pool)
        {
            _stream = Svn.svn_stream_create(baton, pool);
            _readDelegate = null;
            _writeDelegate = null;
            _closeDelegate = null;
        }

        public bool IsNull
        {
            get
            {
                return (_stream == IntPtr.Zero);
            }
        }

        public void ClearPtr()
        {
            _stream = IntPtr.Zero;
        }

        public IntPtr ToIntPtr()
        {
            return _stream;
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnStream stream)
        {
            return stream._stream;
        }

        public static implicit operator SvnStream(IntPtr ptr)
        {
            return new SvnStream(ptr);
        }

        public override string ToString()
        {
            return ("[svn_stream_t:" + _stream.ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Methods wrappers
        public static SvnStream Create(AprPool pool)
        {
            return (new SvnStream(Svn.svn_stream_empty(pool)));
        }

        public static SvnStream Create(IntPtr baton, AprPool pool)
        {
            return (new SvnStream(baton, pool));
        }

        public static SvnStream Create(AprFile file, AprPool pool)
        {
            return (new SvnStream(Svn.svn_stream_from_aprfile2(file, false, pool)));
        }

        public static SvnStream Compress(SvnStream stream, AprPool pool)
        {
            return (new SvnStream(Svn.svn_stream_compressed(stream, pool)));
        }

        public static SvnStream Stdout(AprPool pool)
        {
            IntPtr ptr;
            SvnError err = Svn.svn_stream_for_stdout(out ptr, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
            return (ptr);
        }

        #endregion

        #region Properties wrappers
        public IntPtr Baton
        {
            set
            {
                Svn.svn_stream_set_baton(_stream, value);
            }
        }

        public SvnDelegate ReadDelegate
        {
            get
            {
                return (_readDelegate);
            }
            set
            {
                _readDelegate = value;
                Svn.svn_stream_set_read(_stream, (Svn.svn_read_fn_t) value.Wrapper);
            }
        }

        public SvnDelegate WriteDelegate
        {
            get
            {
                return (_writeDelegate);
            }
            set
            {
                _writeDelegate = value;
                Svn.svn_stream_set_write(_stream, (Svn.svn_write_fn_t) value.Wrapper);
            }
        }

        public SvnDelegate CloseDelegate
        {
            get
            {
                return (_closeDelegate);
            }
            set
            {
                _closeDelegate = value;
                Svn.svn_stream_set_close(_stream, (Svn.svn_close_fn_t) value.Wrapper);
            }
        }
        #endregion
    }
}