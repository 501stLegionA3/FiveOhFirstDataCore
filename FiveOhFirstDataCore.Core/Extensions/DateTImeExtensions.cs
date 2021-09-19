namespace FiveOhFirstDataCore.Data.Extensions
{
    public static class DateTImeExtensions
    {
        private static TimeZoneInfo? TzInternal { get; set; } = null;
        private static TimeZoneInfo TimeZone
        {
            get
            {
                if (TzInternal is null)
                    TzInternal = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

                return TzInternal;
            }
        }

        public static DateTime ToEst(this DateTime utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZone);
        }

        public static int DaysFromToday(this DateTime today)
        {
            return DateTime.UtcNow.Subtract(today).Days;
        }

        public static bool IsAnniversary(this DateTime date1, DateTime date2)
        {
            int date1Month = date1.Month;
            int date1Day = date1.Day;
            int date2Month = date2.Month;
            int date2Day = date2.Day;
            if ((date1Month == date2Month) && (date1Day == date2Day))
            {
                return true;
            }

            return false;
        }
    }
}
