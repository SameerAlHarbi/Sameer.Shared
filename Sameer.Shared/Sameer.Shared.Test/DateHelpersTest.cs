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
    }
}
