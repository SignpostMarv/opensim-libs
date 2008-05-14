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
using System.Runtime.InteropServices;

namespace PumaCode.SvnDotNet.SubversionSharp {
    /// <summary>
    /// A client context structure, which holds client specific callbacks, 
    /// batons, serves as a cache for configuration options, and other various 
    /// and sundry things.
    /// </summary>
    public unsafe class SvnClientContext : IAprUnmanaged {
        private svn_client_ctx_t* _clientContext;
        private SvnAuthBaton _authBaton;
        private SvnDelegate _notifyFunc;
        private SvnDelegate _notifyFunc2;
        private SvnDelegate _logMsgFunc;
        private SvnDelegate _logMsgFunc2;
        private SvnDelegate _cancelFunc;
        private SvnDelegate _progressFunc;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct svn_client_ctx_t {
            public IntPtr auth_baton;
            [Obsolete("Provided for backward compatibility with the 1.1 API")]
            public IntPtr notify_func;
            [Obsolete("Provided for backward compatibility with the 1.1 API")]
            public IntPtr notify_baton;
            [Obsolete("Provided for backward compatibility with the 1.2 API")]
            public IntPtr log_msg_func;
            [Obsolete("Provided for backward compatibility with the 1.2 API")]
            public IntPtr log_msg_baton;
            public IntPtr config;
            public IntPtr cancel_func;
            public IntPtr cancel_baton;
            public IntPtr notify_func2;
            public IntPtr notify_baton2;
            public IntPtr log_msg_func2;
            public IntPtr log_msg_baton2;
            public IntPtr progress_func;
            public IntPtr progress_baton;
        }

        #region Generic embedding functions of an IntPtr
        public SvnClientContext(IntPtr ptr)
        {
            _clientContext = (svn_client_ctx_t*) ptr.ToPointer();
            _authBaton = new SvnAuthBaton();
            _notifyFunc = SvnDelegate.NullFunc;
            _notifyFunc2 = SvnDelegate.NullFunc;
            _logMsgFunc = SvnDelegate.NullFunc;
            _cancelFunc = SvnDelegate.NullFunc;
        }

        public bool IsNull
        {
            get
            {
                return (_clientContext == null);
            }
        }

        private void CheckPtr()
        {
            if(IsNull)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            _clientContext = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(_clientContext);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(SvnClientContext clientContext)
        {
            return new IntPtr(clientContext._clientContext);
        }

        public static implicit operator SvnClientContext(IntPtr ptr)
        {
            return new SvnClientContext(ptr);
        }

        public override string ToString()
        {
            return ("[svn_client_context_t:" + (new IntPtr(_clientContext)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Wrapper methods
        public static SvnClientContext Create(AprPool pool)
        {
            IntPtr ptr;

            Debug.Write(String.Format("svn_client_create_context({0})...", pool));
            SvnError err = Svn.svn_client_create_context(out ptr, pool);
            if(!err.IsNoError)
                throw new SvnException(err);
            Debug.WriteLine(String.Format("Done({0:X})", ((Int32) ptr)));

            return (ptr);
        }
        #endregion

        #region Wrapper Properties
        /// <summary>
        /// main authentication baton.
        /// </summary>
        public SvnAuthBaton AuthBaton
        {
            get
            {
                CheckPtr();
                return (_authBaton);
            }
            set
            {
                CheckPtr();
                _authBaton = value;
                _clientContext->auth_baton = _authBaton;
            }
        }
        /// <summary>
        /// notification callback function.
        /// </summary>
        [Obsolete("Provided for backward compatibility with the 1.1 API")]
        public SvnDelegate NotifyFunc
        {
            get
            {
                CheckPtr();
                return (_notifyFunc);
            }
            set
            {
                CheckPtr();
                _notifyFunc = value;
                _clientContext->notify_func = _notifyFunc.WrapperPtr;
            }
        }
        /// <summary>
        /// notification callback baton
        /// </summary>
        [Obsolete("Provided for backward compatibility with the 1.1 API")]
        public IntPtr NotifyBaton
        {
            get
            {
                CheckPtr();
                return (_clientContext->notify_baton);

            }
            set
            {
                CheckPtr();
                _clientContext->notify_baton = value;
            }
        }
        /// <summary>
        /// Log message callback function.  NULL means that Subversion
        /// should try not attempt to fetch a log message.
        /// </summary>
        [Obsolete("Provided for backward compatibility with the 1.2 API")]
        public SvnDelegate LogMsgFunc
        {
            get
            {
                CheckPtr();
                return (_logMsgFunc);
            }
            set
            {
                CheckPtr();
                _logMsgFunc = value;
                _clientContext->log_msg_func = _logMsgFunc.WrapperPtr;
            }
        }
        /// <summary>
        /// log message callback baton
        /// </summary>
        [Obsolete("Provided for backward compatibility with the 1.2 API")]
        public IntPtr LogMsgBaton
        {
            get
            {
                CheckPtr();
                return (_clientContext->log_msg_baton);

            }
            set
            {
                CheckPtr();
                _clientContext->log_msg_baton = value;
            }
        }
        /// <summary>
        /// hash mapping of configuration file names to SvnConfig's.
        /// For example, the '~/.subversion/config' file's contents should have
        /// the key "config".
        /// May be left unset (or set to NULL) to use the built-in default settings
        /// and not use any configuration.
        /// </summary>
        public AprHash Config
        {
            get
            {
                CheckPtr();
                return (_clientContext->config);
            }
            set
            {
                CheckPtr();
                _clientContext->config = value;
            }
        }

        /// <summary>
        /// a callback to be used to see if the client wishes to cancel the running operation.
        /// </summary>
        public SvnDelegate CancelFunc
        {
            get
            {
                CheckPtr();
                return (_cancelFunc);
            }
            set
            {
                CheckPtr();
                _cancelFunc = value;
                _clientContext->cancel_func = _cancelFunc.WrapperPtr;
            }
        }

        /// <summary>
        /// a baton to pass to the cancellation callback.
        /// </summary>
        public IntPtr CancelBaton
        {
            get
            {
                CheckPtr();
                return (_clientContext->cancel_baton);

            }
            set
            {
                CheckPtr();
                _clientContext->cancel_baton = value;
            }
        }

        /// <summary>
        /// notification callback function.
        /// </summary>
        public SvnDelegate NotifyFunc2
        {
            get
            {
                CheckPtr();
                return (_notifyFunc2);
            }
            set
            {
                CheckPtr();
                _notifyFunc2 = value;
                _clientContext->notify_func2 = _notifyFunc2.WrapperPtr;
            }
        }
        /// <summary>
        /// notification callback baton for NotifyFunc2
        /// </summary>
        public IntPtr NotifyBaton2
        {
            get
            {
                CheckPtr();
                return (_clientContext->notify_baton2);

            }
            set
            {
                CheckPtr();
                _clientContext->notify_baton2 = value;
            }
        }

        /// <summary>
        /// Log message callback function.  NULL means that Subversion
        /// should try not attempt to fetch a log message.
        /// </summary>
        public SvnDelegate LogMsgFunc2
        {
            get
            {
                CheckPtr();
                return (_logMsgFunc2);
            }
            set
            {
                CheckPtr();
                _logMsgFunc2 = value;
                _clientContext->log_msg_func2 = _logMsgFunc2.WrapperPtr;
            }
        }

        /// <summary>
        /// log message callback baton
        /// </summary>
        public IntPtr LogMsgBaton2
        {
            get
            {
                CheckPtr();
                return (_clientContext->log_msg_baton2);

            }
            set
            {
                CheckPtr();
                _clientContext->log_msg_baton2 = value;
            }
        }

        /// <summary>
        /// Notification callback for network progress information.
        /// May be NULL if not used.
        /// </summary>
        public SvnDelegate ProgressFunc
        {
            get
            {
                CheckPtr();
                return (_progressFunc);
            }
            set
            {
                CheckPtr();
                _progressFunc = value;
                _clientContext->progress_func = _progressFunc.WrapperPtr;
            }
        }
        /// <summary>
        /// Callback baton for ProgressFunc
        /// </summary>
        public IntPtr ProgressBaton
        {
            get
            {
                CheckPtr();
                return (_clientContext->progress_baton);

            }
            set
            {
                CheckPtr();
                _clientContext->progress_baton = value;
            }
        }

        #endregion
    }
}