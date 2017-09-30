using System;

namespace NewWordsBot
{
    public interface ITimeProvider
    {
        DateTime InOneMinute();
        DateTime InThirtyMinutes();
        DateTime InOneDay();
        DateTime InForteenDays();
        DateTime InSixtyDays();
    }
}