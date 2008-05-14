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
using System.Runtime.Serialization;

namespace PumaCode.SvnDotNet.SubversionSharp {
    [Serializable]
    public class SvnNullReferenceException : SvnException {
        const int Result = unchecked((int) 0xA0834003);

        public SvnNullReferenceException()
            : base("An null or uninitialized instance was found where a valid instance is expected.")
        {
            HResult = Result;
        }

        public SvnNullReferenceException(string s)
            : base(s)
        {
            HResult = Result;
        }

        public SvnNullReferenceException(string s, Exception innerException)
            : base(s, innerException)
        {
            HResult = Result;
        }

        public SvnNullReferenceException(SvnError error)
            : base(error)
        {
            error.Clear();
        }

        public SvnNullReferenceException(SvnError error, Exception innerException)
            : base(error, innerException)
        {
            error.Clear();
        }

        public SvnNullReferenceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override int AprErr
        {
            get
            {
                return (unchecked(HResult - Result));
            }
        }
    }
}