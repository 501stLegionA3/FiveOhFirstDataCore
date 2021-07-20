using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Extensions
{
    public static class DateTImeExtensions
    {
        private static TimeZoneInfo? TzInternal { get; set; } = null;
        private static TimeZoneInfo TimeZone { get
            {
                if(TzInternal is null)
                    TzInternal = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

                return TzInternal;
            }
        }

        public static DateTime ToEst(this DateTime utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZone);
        }

        public static int DaysFromToday(this DateTime toady)
        {
            return DateTime.UtcNow.Subtract(toady).Days;
        }
    }
}
