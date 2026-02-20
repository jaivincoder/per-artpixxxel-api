using System;


namespace api.artpixxel.repo.Extensions
{
    public static class DateTimeExtension
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public static string GetElapsedTime(this DateTime date)
        {

            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - date.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 120 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            if (delta > 12 * MONTH)
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";

            }

            return "just now";
        }



        public static double GetUnixEpoch(this DateTime dateTime)
        {
            var unixTime = dateTime.ToUniversalTime() -
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return unixTime.TotalSeconds;
        }

        public static string GetRemainingTime(this DateTime date)
        {

            TimeSpan ts = new TimeSpan( date.Ticks - DateTime.UtcNow.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second remaining" : ts.Seconds + " seconds remaining";

            if (delta < 2 * MINUTE)
                return "a minute remaining";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes remaining";

            if (delta < 90 * MINUTE)
                return "an hour remaining";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours remaining";

            if (delta < 48 * HOUR)
                return "one day remaining";

            if (delta < 30 * DAY)
                return ts.Days + " days remaining";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month remaining" : months + " months remaining";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year remaining" : years + " years remianing";

            }
        }



        public static string ToString(this DateTime dateTime, string format, bool useExtendedSpecifiers)
        {
            return useExtendedSpecifiers
                ? dateTime.ToString(format)
                    .Replace("nn", dateTime.Day.ToOccurrenceSuffix().ToLower())
                    .Replace("NN", dateTime.Day.ToOccurrenceSuffix().ToUpper())
                : dateTime.ToString(format);
        }

    }
}
