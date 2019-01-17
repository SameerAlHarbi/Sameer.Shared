﻿using System;
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
            var minutesLimit = (int)(60 * (value - hourLimit));

            var result = new TimeSpan(hourLimit, minutesLimit, 0);
            return result;
        }

        public static double ConvertToDouble(this TimeSpan timeToConvert)
        {
            int hours = timeToConvert.Hours;
            int minutes = timeToConvert.Minutes;

            double result = minutes / (double)60;
            result += hours;
            return result;
        }


    }
}
