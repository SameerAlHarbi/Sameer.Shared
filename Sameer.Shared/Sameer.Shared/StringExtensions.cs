using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sameer.Shared
{
    public static class StringExtensions
    {

        public static string TrimAndLower(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }
            return text.Trim().ToLower();
        }

        public static string TrimAndUpper(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }
            return text.Trim().ToUpper();
        }

        public static DateTime? TryParseToDate(this string dateText, string dateFormat = "yyyy-MM-dd")
        {
            if (string.IsNullOrWhiteSpace(dateText))
            {
                return null;
            }

            DateTime resultDate = DateTime.Today;
            if (DateTime.TryParseExact(dateText, dateFormat, new CultureInfo(name: "en-US"), DateTimeStyles.None, out resultDate))
            {
                return resultDate;
            }
            return null;
        }

        public static IEnumerable<decimal> TryParseToNumbers(this string textNumbers)
        {
            if (string.IsNullOrWhiteSpace(textNumbers))
            {
                return null;
            }

            List<decimal> resultsNumbersList = null;
            List<string> listOfTextNumbers = textNumbers.ToLower().Split(separator: ',').ToList();
            foreach (var textNumber in listOfTextNumbers)
            {
                decimal resultNumber;
                if (decimal.TryParse(textNumber, out resultNumber))
                {
                    if (resultsNumbersList == null)
                    {
                        resultsNumbersList = new List<decimal>();
                    }
                    resultsNumbersList.Add(resultNumber);
                }
            }

            return resultsNumbersList;
        }

    }
}
