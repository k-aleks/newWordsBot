using System;

namespace NewWordsBot
{
    public class TimeProvider : ITimeProvider
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