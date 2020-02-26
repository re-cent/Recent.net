using System;
using System.Collections.Generic;
using System.Text;

namespace RecentLib
{
    public static class Utils
    {
        /// <summary>
        /// Convert an epoch number to a valid  datetime.
        /// </summary>
        /// <param name="epochTime">The epoch number to convert.</param>
        /// <returns>A datetime object containing the date.</returns>
        public static DateTime convertFromEpoch(long epochTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epochTime);
        }

                /// <summary>
        /// Convert a datetime object to epoch time.
        /// </summary>
        /// <param name="dt">The datetime object to convert.</param>
        /// <returns>A long object containing the epoch time.</returns>
        public static long convertToEpoch(DateTime dt)
        {
            return Convert.ToInt64((dt - new DateTime(1970, 1, 1)).TotalSeconds);
        }

    }
}
