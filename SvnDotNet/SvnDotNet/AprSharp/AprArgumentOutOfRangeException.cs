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
    public class AprArgumentOutOfRangeException : AprArgumentException {
        const int Result = unchecked((int) 0xA0651502);

        private object mActualValue;

        public AprArgumentOutOfRangeException()
            : base("Argument is out of range.")
        {
            HResult = Result;
            mActualValue = null;
        }

        public AprArgumentOutOfRangeException(string paramName)
            : base("Argument is out of range.", paramName)
        {
            HResult = Result;
            mActualValue = null;
        }

        public AprArgumentOutOfRangeException(string paramName, string message)
            : base(message, paramName)
        {
            HResult = Result;
            mActualValue = null;
        }

        public AprArgumentOutOfRangeException(string paramName,
                                              object actualValue,
                                              string message)
            : base(message, paramName)
        {
            mActualValue = actualValue;
            HResult = Result;
        }

        public AprArgumentOutOfRangeException(string paramName, int apr_status)
            : base(apr_status, paramName)
        {
            mActualValue = null;
        }

        public AprArgumentOutOfRangeException(string paramName,
                                              object actualValue,
                                              int apr_status)
            : base(apr_status, paramName)
        {
            mActualValue = actualValue;
        }

        public AprArgumentOutOfRangeException(string paramName,
                                              int actualValue,
                                              int minval, int maxval)
            : base("Expect an integer value between " + minval
                                                 + " and " + maxval + ".",
                        paramName)
        {
            mActualValue = (object) actualValue;
        }

        public AprArgumentOutOfRangeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            mActualValue = info.GetString("ActualValue");
        }

        public virtual object ActualValue
        {
            get
            {
                return mActualValue;
            }
        }

        public override string Message
        {
            get
            {
                string baseMessage = base.Message;
                if(mActualValue == null)
                    return baseMessage;

                return (baseMessage + Environment.NewLine + mActualValue);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ActualValue", mActualValue);
        }
    }
}