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
using System.Runtime.Serialization;

namespace PumaCode.SvnDotNet.AprSharp {
    [Serializable]
    public class AprArgumentException : AprException {
        const int Result = unchecked((int) 0xA0650057);

        private string mParamName;

        public AprArgumentException()
            : base("An invalid argument was specified.")
        {
            HResult = Result;
            mParamName = null;
        }

        public AprArgumentException(string message)
            : base(message)
        {
            HResult = Result;
            mParamName = null;
        }

        public AprArgumentException(string message, string paramName)
            : base(message)
        {
            HResult = Result;
            this.mParamName = paramName;
        }

        public AprArgumentException(int apr_status)
            : base(apr_status)
        {
            mParamName = null;
        }

        public AprArgumentException(int apr_status, string paramName)
            : base(apr_status)
        {
            mParamName = paramName;
        }

        public AprArgumentException(int apr_status, Exception innerException)
            : base(apr_status, innerException)
        {
            mParamName = null;
        }

        public AprArgumentException(int apr_status, string paramName, Exception innerException)
            : base(apr_status, innerException)
        {
            mParamName = paramName;
        }

        public AprArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            mParamName = info.GetString("ParamName");
        }

        public virtual string ParamName
        {
            get
            {
                return mParamName;
            }
        }

        public override string Message
        {
            get
            {
                string baseMessage = base.Message;
                if(baseMessage == null)
                    baseMessage = "An invalid argument was specified.";

                if(mParamName == null)
                    return baseMessage;
                else
                    return (baseMessage + Environment.NewLine
                            + "Parameter name: " + mParamName);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ParamName", mParamName);
        }
    }
}