using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sameer.Shared.Helpers;
using System;

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

        [TestMethod]
        public void TestConvertToGregorianDate_Fail()
        {
            int day = 31;
            int month = 2;
            int year = 1440;

            DateTime result = DateHelpers.ConvertToGregorianDate(year, month, day);

            Assert.IsTrue(result.Year == 2018 && result.Month == 10 && result.Day == 27);
        }
    }
}
