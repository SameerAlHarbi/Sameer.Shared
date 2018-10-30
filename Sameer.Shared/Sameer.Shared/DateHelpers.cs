using System;
using System.Globalization;

namespace Sameer.Shared
{
    public static class DateHelpers
    {

        public static string GetDayName(this DateTime date, bool showDayNameInArabic = false)
        {
            if (!showDayNameInArabic)
            {
                return date.DayOfWeek.ToString();
            }

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "الأحد";
                case DayOfWeek.Monday:
                    return "الإثنين";
                case DayOfWeek.Tuesday:
                    return "الثلاثاء";
                case DayOfWeek.Wednesday:
                    return "الأربعاء";
                case DayOfWeek.Thursday:
                    return "الخميس";
                case DayOfWeek.Friday:
                    return "الجمعة";
                default:
                    return "السبت";
            }
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

        public static (int year, int month ,int day,DayOfWeek dayOfWeek,string arabicDayName) GetInfo(this DateTime date, bool hijri = false)
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
                date.GetDayName(true));
        }

    }
}
