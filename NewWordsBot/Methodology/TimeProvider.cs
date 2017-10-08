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
    
    public class TimeProviderForTests : ITimeProvider
    {
        public DateTime InOneMinute()
        {
            return DateTime.UtcNow;
        }

        public DateTime InThirtyMinutes()
        {
            return DateTime.UtcNow.AddMinutes(2);
        }

        public DateTime InOneDay()
        {
            return DateTime.UtcNow.AddMinutes(3);
        }

        public DateTime InForteenDays()
        {
            return DateTime.UtcNow.AddMinutes(5);
        }

        public DateTime InSixtyDays()
        {
            return DateTime.UtcNow.AddMinutes(10);
        }
    }
}