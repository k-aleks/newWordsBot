using System;

namespace NewWordsBot
{
    internal interface ITimeProvider
    {
        DateTime InOneMinute();
        DateTime InThirtyMinutes();
        DateTime InOneDay();
        DateTime InForteenDays();
        DateTime InSixtyDays();
    }

    class TimeProvider : ITimeProvider
    {
        public DateTime InOneMinute()
        {
            return DateTime.UtcNow.AddMinutes(1);
        }

        public DateTime InThirtyMinutes()
        {
            return DateTime.UtcNow.AddMinutes(30);
        }

        public DateTime InOneDay()
        {
            return DateTime.UtcNow.AddDays(1);
        }

        public DateTime InForteenDays()
        {
            return DateTime.UtcNow.AddDays(14);
        }

        public DateTime InSixtyDays()
        {
            return DateTime.UtcNow.AddDays(60);
        }
    }
}