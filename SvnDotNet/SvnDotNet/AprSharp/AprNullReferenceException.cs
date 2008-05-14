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
    public class AprNullReferenceException : AprException {
        const int Result = unchecked((int) 0xA0654003);

        public AprNullReferenceException()
            : base("An null or uninitialized instance was found where a valid instance is expected.")
        {
            HResult = Result;
        }

        public AprNullReferenceException(string s)
            : base(s)
        {
            HResult = Result;
        }

        public AprNullReferenceException(string s, Exception innerException)
            : base(s, innerException)
        {
            HResult = Result;
        }

        public AprNullReferenceException(int apr_status)
            : base(apr_status)
        {
        }

        public AprNullReferenceException(int apr_status, Exception innerException)
            : base(apr_status, innerException)
        {
        }

        public AprNullReferenceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}