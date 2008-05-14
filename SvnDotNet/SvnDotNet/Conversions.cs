using System;
using System.Collections.Generic;
using System.Text;
using PumaCode.SvnDotNet.SubversionSharp;

namespace PumaCode.SvnDotNet {
    internal static class Conversions {
        /// <summary>
        /// Number of ticks corresponding to 00:00:00 January 1, 1970 UTC
        /// </summary>
        private const long EpochTicks = 621355968000000000L;

        /// <summary>
        /// Converts a DateTime to an Int64 representing an apr_time_t value, which is
        /// "Number of microseconds since 00:00:00 january 1, 1970 UTC". If
        /// <paramref name="date"/> is earlier than this date, returns 0.
        /// </summary>
        internal static long DateTimeToAprDate(DateTime date)
        {
            if(date.Ticks < EpochTicks)
                return 0;

            return (date.Ticks - EpochTicks) / 10;
        }

        /// <summary>
        /// Converts an Int64 representing an apr_time_t value (Number of microseconds
        /// since 00:00:00 january 1, 1970 UTC) into the corresponding DateTime. As a
        /// special case, an <paramref name="aprDate"/> of 0 returns Date.MinValue.
        /// </summary>
        internal static DateTime AprDateToDateTime(long aprDate)
        {
            if(aprDate == 0)
                return DateTime.MinValue;

            return new DateTime(aprDate * 10 - EpochTicks);
        }

        internal static string SvnDataToString(SvnData svnData)
        {
            return svnData.IsNull? null : svnData.ToString();
        }

    }
}
