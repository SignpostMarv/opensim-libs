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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PumaCode.SvnDotNet.AprSharp {
    public class AprTime {
        private AprTime()
        {
        }

        public const int APR_RFC822_DATE_LEN = 30;
        public const int APR_CTIME_LEN = 25;

        public static long Now()
        {
            return (Apr.apr_time_now());
        }

        public static string Rfc822Date(long value)
        {
            StringBuilder buf = new StringBuilder(APR_RFC822_DATE_LEN);
            int res = Apr.apr_rfc822_date(buf, value);
            if(res != 0)
                throw new AprException(res);
            return (buf.ToString());
        }

        public static string CTime(long value)
        {
            StringBuilder buf = new StringBuilder(APR_CTIME_LEN);
            int res = Apr.apr_ctime(buf, value);
            if(res != 0)
                throw new AprException(res);
            return (buf.ToString());
        }

        public static long FromDateTime(DateTime dt)
        {
            if(dt.Ticks < 621355968000000000)
                throw new AprException("A DateTime prior to 1/1/1970 is not convertible to AprTime");
            return ((dt.Ticks / 10) - 62135596800000000);
        }

        public static DateTime ToDateTime(long at)
        {
            return new DateTime((at + 62135596800000000) * 10);
        }
    }
}