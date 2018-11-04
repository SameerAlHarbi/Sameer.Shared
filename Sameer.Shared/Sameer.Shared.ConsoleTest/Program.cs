using System;
using System.Globalization;

namespace Sameer.Shared.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo arCulture = new CultureInfo("ar-SA");
            arCulture.DateTimeFormat.Calendar = new UmAlQuraCalendar();

            foreach (var item in arCulture.DateTimeFormat.MonthNames)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
            Console.WriteLine();

            foreach (var item in arCulture.DateTimeFormat.DayNames)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
            Console.WriteLine();

            CultureInfo enCulture = new CultureInfo("en-US");
            arCulture.DateTimeFormat.Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);

            foreach (var item in enCulture.DateTimeFormat.MonthNames)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
            Console.WriteLine();

            foreach (var item in enCulture.DateTimeFormat.DayNames)
            {
                Console.WriteLine(item);
            }

            Console.ReadKey();
        }
    }
}
