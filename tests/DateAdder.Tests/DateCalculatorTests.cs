namespace DateAdder.Tests
{
    public class DateCalculatorTests
    {
        [Theory]
        [InlineData(2016, true)]
        [InlineData(2000, true)]   // div by 400
        [InlineData(1900, false)]  // div by 100, not 400
        [InlineData(2100, false)]
        [InlineData(2015, false)]
        [InlineData(2400, true)]
        public void IsLeapYear_MatchesGregorianRules(int year, bool expected)
        {
            Assert.Equal(expected, DateCalculator.IsLeapYear(year));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10000)]
        public void IsLeapYear_OutOfRangeYearThrows(int year)
        {
            var exception = Record.Exception(() => DateCalculator.IsLeapYear(year));
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Theory]
        [InlineData(2, 2016, 29)]
        [InlineData(2, 2015, 28)]
        [InlineData(2, 1900, 28)]
        [InlineData(2, 2000, 29)]
        [InlineData(4, 2021, 30)]
        [InlineData(1, 2021, 31)]
        public void DaysInMonth_ReturnsCorrectCount(int month, int year, int expectedDays)
        {
            Assert.Equal(expectedDays, DateCalculator.DaysInMonth(month, year));
        }

        [Fact]
        public void AddDays_SpecExample_MatchesExpectedOutput()
        {
            // 31/01/2016 + 1 = 01/02/2016
            var result = DateCalculator.AddDays(31, 1, 2016, 1);
            Assert.Equal((1, 2, 2016), result);
        }

        [Fact]
        public void AddDays_ZeroDays_ReturnsSameDate()
        {
            var result = DateCalculator.AddDays(15, 6, 2020, 0);
            Assert.Equal((15, 6, 2020), result);
        }

        [Fact]
        public void AddDays_RollsIntoLeapDay()
        {
            var result = DateCalculator.AddDays(28, 2, 2016, 1);
            Assert.Equal((29, 2, 2016), result);
        }

        [Fact]
        public void AddDays_NonLeapYearSkipsFeb29()
        {
            var result = DateCalculator.AddDays(28, 2, 2015, 1);
            Assert.Equal((1, 3, 2015), result);
        }

        [Fact]
        public void AddDays_CenturyNonLeapYearSkipsFeb29()
        {
            var result = DateCalculator.AddDays(28, 2, 1900, 1);
            Assert.Equal((1, 3, 1900), result);
        }

        [Fact]
        public void AddDays_CenturyLeapYearIncludesFeb29()
        {
            var result = DateCalculator.AddDays(28, 2, 2000, 1);
            Assert.Equal((29, 2, 2000), result);
        }

        [Fact]
        public void AddDays_RollsOverYearBoundary()
        {
            var result = DateCalculator.AddDays(31, 12, 2016, 1);
            Assert.Equal((1, 1, 2017), result);
        }

        [Fact]
        public void AddDays_FullLeapYearCycleLandsOnDec31()
        {
            // 2020 is a leap year, 366 days
            var result = DateCalculator.AddDays(1, 1, 2020, 365);
            Assert.Equal((31, 12, 2020), result);
        }

        [Fact]
        public void AddDays_NegativeDaysSubtractsAcrossLeapBoundary()
        {
            var result = DateCalculator.AddDays(1, 3, 2016, -1);
            Assert.Equal((29, 2, 2016), result);
        }

        [Fact]
        public void AddDays_NegativeDaysRollsBackAcrossYearBoundary()
        {
            var result = DateCalculator.AddDays(1, 1, 2017, -1);
            Assert.Equal((31, 12, 2016), result);
        }

        [Fact]
        public void AddDays_LargeAdditionSpansMultipleYears()
        {
            var result = DateCalculator.AddDays(1, 1, 2020, 1000);
            Assert.Equal((27, 9, 2022), result);
        }

        [Fact]
        public void AddDays_YearDroppingBelowMinimumThrows()
        {
            var exception = Record.Exception(() => DateCalculator.AddDays(1, 1, 1, -1));
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Fact]
        public void AddDays_YearRisingAbove9999Throws()
        {
            var exception = Record.Exception(() => DateCalculator.AddDays(31, 12, 9999, 1));
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Fact]
        public void AddDays_InvalidStartingDayThrows()
        {
            // Feb has no 30th
            var exception = Record.Exception(() => DateCalculator.AddDays(30, 2, 2021, 1));
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }
    }
}
