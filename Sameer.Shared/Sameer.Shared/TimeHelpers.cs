using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sameer.Shared
{
    public static class TimeHelpers
    {

        public static DateTime LastTimeOfDay(this DateTime workDate)
        {
            return new DateTime(workDate.Year, workDate.Month, workDate.Day, 23, 59, 59, 999);
        }

        public static DateTime CalcDateLimit(this DateTime workDate, double timeLimit, bool maxTime = false)
        {
            var limit = ConvertToTime(timeLimit);

            DateTime result;
            if (!maxTime)//minimum time
            {
                result = workDate.Subtract(limit);
            }
            else
            {
                result = workDate.Add(limit);
            }

            return result;
        }

        public static TimeSpan CalcTimeLimit(this TimeSpan workTime, double timeLimit, bool maxTime = false)
        {
            var limit = ConvertToTime(timeLimit);

            TimeSpan result;
            if (!maxTime)//minimum time
            {
                result = workTime.Subtract(limit);
            }
            else
            {
                result = workTime.Add(limit);
            }

            return result;
        }

        public static TimeSpan ConvertToTime(this double value)
        {
            var hourLimit = (int)value;
            var minutes = 60 * (value - hourLimit);
            var minutesLimit = (int)minutes;
            var seconds = minutes - minutesLimit;
            var secondsLong = 60 * seconds;
            var secondsLimit = (int)Math.Round(secondsLong);
            if(secondsLimit >= 60)
            {
                secondsLimit = 0;
                minutesLimit++;
                if(minutesLimit >= 60)
                {
                    minutesLimit = 0;
                    hourLimit++;
                }
            }

            return new TimeSpan(hourLimit, minutesLimit, secondsLimit);
        }

        public static double ConvertToDouble(this TimeSpan timeToConvert)
        {
            int hours = timeToConvert.Hours;
            int minutes = timeToConvert.Minutes;
            int seconds = timeToConvert.Seconds;

            double minutesResult = minutes / 60d;
            double secondsResult = seconds / 3600d;

            return hours + minutesResult + secondsResult;
        }


    }
}
