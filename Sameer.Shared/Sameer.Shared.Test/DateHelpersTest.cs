using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sameer.Shared;

namespace Sameer.Shared.Test
{
    [TestClass]
    public class DateHelpersTest
    {
        [TestMethod]
        public void TestConvertToGregorianDate_Success()
        {
            int day = 18;
            int month = 2;
            int year = 1440;

            DateTime result = DateHelpers.ConvertToGregorianDate(year, month, day);

            Assert.IsTrue(result.Year == 2018 && result.Month == 10 && result.Day == 27);
        }

        //[TestMethod]
        //public void TestConvertToGregorianDate_Fail()
        //{
        //    int day = 31;
        //    int month = 2;
        //    int year = 1440;

        //    DateTime result = DateHelpers.ConvertToGregorianDate(year, month, day);

        //    Assert.IsFalse(result.Year == 2018 && result.Month == 10 && result.Day == 27);
        //}

        [TestMethod]
        public void TestDateInfo_Success()
        {
            DateTime dt = new DateTime(2018, 10, 30);
            var result = dt.GetInfo(true);

            Assert.IsTrue(result.year == 1440
                && result.month == 2
                && result.day == 21 
                && result.dayOfWeek == DayOfWeek.Tuesday
                && result.arabicDayName== "«·À·«À«¡");
        }

        [TestMethod]
        public void TestMonthName_GregorianEnglish()
        {
            DateTime dt = new DateTime(2018, 10, 30);
            var result = dt.GetMonthName(false);

            Assert.IsTrue(result == "October");
        }

        [TestMethod]
        public void TestMonthName_HijriEnglish()
        {
            DateTime dt = new DateTime(2018, 10, 30);
            var result = dt.GetMonthName(true);

            Assert.IsTrue(result == "October");
        }

        [TestMethod]
        public void TestConvertToDouble_Success()
        {
            TimeSpan tm = new TimeSpan(2, 15, 34);
            double results = tm.ConvertToDouble();
            Assert.IsTrue(results  == 2.2594444444444446d);
        }

        [TestMethod]
        public void TestConvertToTime_Success()
        {
            double tmDouble =  2.2594444444444446d;
            TimeSpan results = tmDouble.ConvertToTime();
            Assert.IsTrue(results.Hours == 2 && results.Minutes == 15 && results.Seconds == 34 );
        }

        [TestMethod]
        public void TestConvertToTime2_Success()
        {
            double tmDouble = 18.766666666666666d;
            TimeSpan results = tmDouble.ConvertToTime();
            Assert.IsTrue(results.Hours == 2 && results.Minutes == 15 && results.Seconds == 34 );
        }
    }
}
