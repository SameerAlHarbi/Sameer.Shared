using System;
using System.Globalization;

namespace Sameer.Shared
{
    public static class DateHelpers
    {

        public static string GetDayName(this DayOfWeek dayOfWeek, bool arabic = false)
        {
            return CultureInfo
                .GetCultureInfo(arabic ? "ar-SA" : "en-US")
                .DateTimeFormat.GetDayName(dayOfWeek);
        }

        public static string GetDayName(this DateTime date, bool arabic = false)
        {
            return date.DayOfWeek.GetDayName(arabic);
        }

        public static string ConvertToString(this DateTime date, bool hijri = false
            , bool showDayName = false, bool showDayNameInArabic = false
            , string format = "yyyy/MM/dd")
        {
            CultureInfo culture = new CultureInfo(hijri ? "ar-SA" : "en-US");

            if (hijri)
            {
                culture.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            }
            else
            {
                culture.DateTimeFormat.Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            }

            var resultDateString = date.ToString(format, culture);

            if (showDayName)
            {
                return  $"{date.GetDayName(showDayNameInArabic)} {resultDateString}";
            }

            return resultDateString;
        }

        public static DateTime ConvertToDate(this string dateString, bool hijriDateString = false, string format = "yyyy/MM/dd")
        {
            if (!DateTime.TryParseExact(dateString, format, new CultureInfo(hijriDateString ? "ar-SA" : "en-US"), DateTimeStyles.None, out DateTime resultDate))
            {
                throw new Exception("Invalid date.");
            }
            return resultDate;
        }

        public static DateTime ConvertToGregorianDate(int hijriYear,int hijriMonth,int hijriDay)
        {
            if ((hijriDay < 1) || (hijriDay > 30))
            {
                throw new Exception("Invalid hijri day value not between 1 and 30");
            }
            if ((hijriMonth < 1) || (hijriMonth > 12))
            {
                throw new Exception("Invalid hijri month value not between 1 and 12");
            }
            if ((hijriYear < 1300) || (hijriYear > 2000))
            {
                throw new Exception("Invalid hijri year value");
            }
            CultureInfo arCul = new CultureInfo("ar-SA");
            arCul.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            string dateString = string.Format("{0:0000}/{1:00}/{2:00}", hijriYear, hijriMonth, hijriDay);
            return dateString.ConvertToDate(true);
        }

        public static int GetYear(this DateTime date,bool hijri=false)
        {
            CultureInfo culture = new CultureInfo(hijri? "ar-SA" : "en-US");

            if (hijri)
            {
                culture.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            }
            else
            {
                culture.DateTimeFormat.Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            }

            return int.Parse(date.ToString("yyyy", culture));
        }

        public static int GetMonth(this DateTime date, bool hijri = false)
        {
            CultureInfo culture = new CultureInfo(hijri? "ar-SA" : "en-US");

            if (hijri)
            {
                culture.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            }
            else
            {
                culture.DateTimeFormat.Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            }

            return int.Parse(date.ToString("MM", culture));
        }

        public static int GetDay(this DateTime date, bool hijri = false)
        {
            CultureInfo culture = new CultureInfo(hijri ? "ar-SA" : "en-US");

            if (hijri)
            {
                culture.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            }
            else
            {
                culture.DateTimeFormat.Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            }

            return int.Parse(date.ToString("dd", culture));
        }

        //public static string TranslateMonthName(string monthName)
        //{
        //    switch (monthName)
        //    {
        //        case "Ju"
        //    }
        //}

        //public static string GetMonthName(int monthNumber, bool hijri = false, bool arabicName = false)
        //{
        //    CultureInfo culture = new CultureInfo(hijri ? "ar-SA" : "en-US");

        //    if (hijri)
        //    {
        //        culture.DateTimeFormat.Calendar = new UmAlQuraCalendar();
        //    }
        //    else
        //    {
        //        culture.DateTimeFormat.Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
        //    }

        //    string results = culture.DateTimeFormat.GetMonthName(monthNumber);

        //    switch(results)
        //    {

        //    }
        //}

        public static string GetMonthName(this DateTime date,bool hijri=false)
        {
            CultureInfo culture = new CultureInfo(hijri ? "ar-SA" : "en-US");

            if (hijri)
            {
                culture.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            }
            else
            {
                culture.DateTimeFormat.Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            }

            return date.ToString("MMMM", culture);
        }

        public static (int year, int month ,int day,DayOfWeek dayOfWeek,string arabicDayName, string monthName) GetInfo(this DateTime date, bool hijri = false)
        {
            CultureInfo culture = new CultureInfo(hijri ? "ar-SA" : "en-US");

            if (hijri)
            {
                culture.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            }
            else
            {
                culture.DateTimeFormat.Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            }

            return (int.Parse(date.ToString("yyyy", culture)),
                int.Parse(date.ToString("MM", culture)),
                int.Parse(date.ToString("dd", culture)),
                date.DayOfWeek,
                date.DayOfWeek.GetDayName(true),
                date.ToString("MMMM", culture));
        }

        public static int CalcDuration(this DateTime startDate, DateTime endDate)
        {
            if (endDate.Date < startDate.Date)
            {
                throw new Exception("Error: endDate can't be less than startDate");
            }
            return endDate.Date.Subtract(startDate.Date).Days + 1;
        }

        public static bool IsBetween(this DateTime date
            , DateTime startDate
            , DateTime endDate
            , bool compareTime = true)
        {
            return compareTime ? date >= startDate && date <= endDate 
                               : date.Date >= startDate.Date && date.Date <= endDate.Date;
        }

        public static bool IsConflictRange(this DateTime startDate, DateTime endDate,
            DateTime newStartDate, DateTime newEndDate, bool compareTime = true)
        {

           return startDate.IsBetween(newStartDate, newEndDate, compareTime) 
                || newStartDate.IsBetween(startDate, endDate,compareTime);

        }


    }
}
