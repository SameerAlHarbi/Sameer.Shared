using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Sameer.Shared.Helpers
{
    public static class DateHelpers
    {

        public static string ConvertToString(this DateTime date, bool hijri = false
            , bool showDayName = false, bool showDayNameInArabic = false
            , string format = "yyyy/MM/dd")
        {
            if(showDayName)
            {
                return  $"{date.GetDayName(showDayNameInArabic)} {date.ConvertToString(hijri, format)}";
            }

            return date.ConvertToString(hijri,format);
        }

        public static string ConvertToString(this DateTime date,bool hijri = false
            , string format = "yyyy/MM/dd")
        {
            CultureInfo culture = new CultureInfo(hijri? "ar-SA":"en-US");

            if(hijri)
            {
                culture.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            }
            else
            {
                culture.DateTimeFormat.Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            }
            
            return date.ToString(format, culture);
        }

        public static string GetDayName(this DateTime date, bool showDayNameInArabic = false)
        {
            if(!showDayNameInArabic)
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

        public static DateTime ConvertToDate(this string dateString, bool hijriDateString = false, string format = "yyyy/MM/dd")
        {
            if (!DateTime.TryParseExact(dateString, format, new CultureInfo(hijriDateString ? "ar-SA" : "en-US"), DateTimeStyles.None, out DateTime resultDate))
            {
                throw new Exception("Invalid date.");
            }
            return resultDate;
        }

        public static DateTime ConvertToGregorianDate([Range(1300,1500,ErrorMessage = "Invalid hijri year !")]int hijriYear, [Range(1, 12, ErrorMessage = "Invalid hijri month !")]int hijriMonth, [Range(1,30, ErrorMessage = "Invalid hijri day !")]int hijriDay)
        {
            CultureInfo arCul = new CultureInfo("ar-SA");
            arCul.DateTimeFormat.Calendar = new UmAlQuraCalendar();
            string dateString = string.Format("{0:0000}/{1:00}/{2:00}", hijriYear, hijriMonth, hijriDay);
            return dateString.ConvertToDate(true);
        }
    }
}
