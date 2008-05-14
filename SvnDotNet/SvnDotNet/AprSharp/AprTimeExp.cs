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
    public unsafe struct AprTimeExp : IAprUnmanaged {
        private apr_time_exp_t* mTimeExp;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct apr_time_exp_t {
            public int tm_usec;
            public int tm_sec;
            public int tm_min;
            public int tm_hour;
            public int tm_mday;
            public int tm_mon;
            public int tm_year;
            public int tm_wday;
            public int tm_yday;
            public int tm_isdst;
            public int tm_gmtoff;
        }

        #region Generic embedding functions of an IntPtr
        private AprTimeExp(apr_time_exp_t* ptr)
        {
            mTimeExp = ptr;
        }

        public AprTimeExp(out GCHandle handle)
        {
            handle = GCHandle.Alloc(new apr_time_exp_t(), GCHandleType.Pinned);
            mTimeExp = (apr_time_exp_t*) handle.AddrOfPinnedObject().ToPointer();
        }

        public AprTimeExp(IntPtr ptr)
        {
            mTimeExp = (apr_time_exp_t*) ptr.ToPointer();
        }

        public bool IsNull
        {
            get
            {
                return (mTimeExp == null);
            }
        }

        private void CheckPtr()
        {
            if(mTimeExp == null)
                throw new AprNullReferenceException();
        }

        public void ClearPtr()
        {
            mTimeExp = null;
        }

        public IntPtr ToIntPtr()
        {
            return new IntPtr(mTimeExp);
        }

        public bool ReferenceEquals(IAprUnmanaged obj)
        {
            return (obj.ToIntPtr() == ToIntPtr());
        }

        public static implicit operator IntPtr(AprTimeExp timeExp)
        {
            return new IntPtr(timeExp.mTimeExp);
        }

        public static implicit operator AprTimeExp(IntPtr ptr)
        {
            return new AprTimeExp(ptr);
        }

        public override string ToString()
        {
            return ("[apr_time_exp_t:" + (new IntPtr(mTimeExp)).ToInt32().ToString("X") + "]");
        }
        #endregion

        #region Methods wrappers
        public static AprTimeExp Alloc(AprPool pool)
        {
            return (new AprTimeExp((apr_time_exp_t*) pool.CAlloc(sizeof(apr_time_exp_t))));
        }

        public static AprTimeExp Alloc(out GCHandle handle)
        {
            return (new AprTimeExp(out handle));
        }

        public void SetTimeTZ(long value, int tz)
        {
            Debug.Write(String.Format("apr_time_exp_tz({0:X},{1},{2})...", (new IntPtr(mTimeExp)).ToInt32(), value, tz));
            int res = Apr.apr_time_exp_tz(new IntPtr(mTimeExp), value, tz);
            if(res != 0)
                throw new AprException(res);
            Debug.WriteLine("Done");
        }

        public string ToString(string format)
        {
            return (ToString(format, 256));
        }

        public string ToString(string format, int size)
        {
            StringBuilder str = new StringBuilder(size);
            uint len;
            Debug.Write(String.Format("apr_strftime({0:X},{1})...", (new IntPtr(mTimeExp)).ToInt32(), format));
            int res = Apr.apr_strftime(str, out len, (uint) str.Capacity, format, new IntPtr(mTimeExp));
            if(res != 0)
                throw new AprException(res);
            Debug.WriteLine("Done");
            return (str.ToString());
        }

        public long Time
        {
            get
            {
                long time;
                Debug.Write(String.Format("apr_time_exp_get({0:X})...", (new IntPtr(mTimeExp)).ToInt32()));
                int res = Apr.apr_time_exp_get(out time, new IntPtr(mTimeExp));
                if(res != 0)
                    throw new AprException(res);
                Debug.WriteLine(String.Format("Done({0})", time));
                return (time);
            }
            set
            {
                CheckPtr();
                Debug.Write(String.Format("apr_time_exp_gmt({0:X},{1})...", (new IntPtr(mTimeExp)).ToInt32(), value));
                int res = Apr.apr_time_exp_gmt(new IntPtr(mTimeExp), value);
                if(res != 0)
                    throw new AprException(res);
                Debug.WriteLine("Done");
            }
        }

        public long GmtTime
        {
            get
            {
                CheckPtr();
                long time;
                Debug.Write(String.Format("apr_time_exp_gmt_get({0:X})...", (new IntPtr(mTimeExp)).ToInt32()));
                int res = Apr.apr_time_exp_gmt_get(out time, new IntPtr(mTimeExp));
                if(res != 0)
                    throw new AprException(res);
                Debug.WriteLine(String.Format("Done({0})", time));
                return (time);
            }
            set
            {
                CheckPtr();
                Debug.Write(String.Format("apr_time_exp_lt({0:X},{1})...", (new IntPtr(mTimeExp)).ToInt32(), value));
                int res = Apr.apr_time_exp_lt(new IntPtr(mTimeExp), value);
                if(res != 0)
                    throw new AprException(res);
                Debug.WriteLine("Done");
            }
        }
        #endregion

        #region Structure members wrapper (Properties)
        public int MicroSeconds
        {
            get
            {
                CheckPtr();
                return (mTimeExp->tm_usec);
            }
            set
            {
                CheckPtr();
                if(value < 0 || value > 999999)
                    throw new AprArgumentOutOfRangeException("MicroSeconds", value, 0, 999999);
                mTimeExp->tm_usec = value;
            }
        }

        public int Seconds
        {
            get
            {
                CheckPtr();
                return (mTimeExp->tm_sec);
            }
            set
            {
                CheckPtr();
                if(value < 0 || value > 59)
                    throw new AprArgumentOutOfRangeException("Seconds", value, 0, 59);
                mTimeExp->tm_sec = value;
            }
        }

        public int Minutes
        {
            get
            {
                CheckPtr();
                return (mTimeExp->tm_min);
            }
            set
            {
                CheckPtr();
                if(value < 0 || value > 59)
                    throw new AprArgumentOutOfRangeException("Minutes", value, 0, 59);
                mTimeExp->tm_min = value;
            }
        }

        public int Hours
        {
            get
            {
                CheckPtr();
                return (mTimeExp->tm_hour);
            }
            set
            {
                CheckPtr();
                if(value < 0 || value > 23)
                    throw new AprArgumentOutOfRangeException("Hours", value, 0, 23);
                mTimeExp->tm_hour = value;
            }
        }

        public int Day
        {
            get
            {
                CheckPtr();
                return (mTimeExp->tm_mday);
            }
            set
            {
                CheckPtr();
                if(value < 1 || value > 31)
                    throw new AprArgumentOutOfRangeException("Day", value, 1, 31);
                mTimeExp->tm_mday = value;
            }
        }

        public int Month
        {
            get
            {
                CheckPtr();
                return (mTimeExp->tm_mon + 1);
            }
            set
            {
                CheckPtr();
                if(value < 1 || value > 12)
                    throw new AprArgumentOutOfRangeException("Month", value, 1, 12);
                mTimeExp->tm_mon = value - 1;
            }
        }

        public int Year
        {
            get
            {
                CheckPtr();
                return (mTimeExp->tm_year + 1900);
            }
            set
            {
                CheckPtr();
                if(value < 1970)
                    throw new AprArgumentOutOfRangeException("Year", value, "Expect an integer value over 1900.");
                mTimeExp->tm_year = value - 1900;
            }
        }

        public int WeekDay
        {
            get
            {
                CheckPtr();
                return (mTimeExp->tm_wday);
            }
        }

        public int YearDay
        {
            get
            {
                CheckPtr();
                int m = Month;
                int y = Year;
                y = (y % 4 == 0 && (y % 100 != 0 || y % 400 == 0)) ? 1 : 0;
                return (Day + ((979 * (m - 12 * ((m - 14) / 12)) - 2918) >> 5) + y - 307 + (365 * ((m + 9) / 12)));
                //return(mTimeExp->tm_yday);
            }
        }

        public bool IsDaylightSaving
        {
            get
            {
                CheckPtr();
                return (mTimeExp->tm_isdst != 0);
            }
            set
            {
                CheckPtr();
                mTimeExp->tm_isdst = (value) ? 1 : 0;
            }
        }

        public int TimeZone
        {
            get
            {
                CheckPtr();
                return (mTimeExp->tm_gmtoff);
            }
            set
            {
                CheckPtr();
                if(value < -43200 || value > 43200)
                    throw new AprArgumentOutOfRangeException("GmtOffset", value, -43200, +43200);
                mTimeExp->tm_gmtoff = value;
            }
        }
        #endregion
    }
}