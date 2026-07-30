using DateAdder.Models;
using Moq;
using Microsoft.Extensions.Logging;

namespace DateAdder.Tests
{
    public class DateCalculatorTests
    {
        private readonly Mock<ILogger<DateCalculator>> _loggerMock;
        private readonly DateCalculator _calculator;

        public DateCalculatorTests()
        {
            _loggerMock = new Mock<ILogger<DateCalculator>>();
            _calculator = new DateCalculator(_loggerMock.Object);
        }

        [Theory]
        [InlineData(2016, true)]
        [InlineData(2000, true)]   // div by 400
        [InlineData(1900, false)]  // div by 100, not 400
        [InlineData(2100, false)]
        [InlineData(2015, false)]
        [InlineData(2400, true)]
        public void IsLeapYear_MatchesGregorianRules(int year, bool expected)
        {
            Assert.Equal(expected, _calculator.IsLeapYear(year));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10000)]
        public void IsLeapYear_OutOfRangeYearThrows(int year)
        {
            var exception = Record.Exception(() => _calculator.IsLeapYear(year));
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
            Assert.Equal(expectedDays, _calculator.DaysInMonth(month, year));
        }

        [Fact]
        public void AddDays_SpecExample_MatchesExpectedOutput()
        {
            // 31/01/2016 + 1 = 01/02/2016
            var result = _calculator.AddDays(31, 1, 2016, 1);
            Assert.Equal(new DateModel(1, 2, 2016), result);
        }

        [Fact]
        public void AddDays_ZeroDays_ReturnsSameDate()
        {
            var result = _calculator.AddDays(15, 6, 2020, 0);
            Assert.Equal(new DateModel(15, 6, 2020), result);
        }

        [Fact]
        public void AddDays_RollsIntoLeapDay()
        {
            var result = _calculator.AddDays(28, 2, 2016, 1);
            Assert.Equal(new DateModel(29, 2, 2016), result);
        }

        [Fact]
        public void AddDays_NonLeapYearSkipsFeb29()
        {
            var result = _calculator.AddDays(28, 2, 2015, 1);
            Assert.Equal(new DateModel(1, 3, 2015), result);
        }

        [Fact]
        public void AddDays_CenturyNonLeapYearSkipsFeb29()
        {
            var result = _calculator.AddDays(28, 2, 1900, 1);
            Assert.Equal(new DateModel(1, 3, 1900), result);
        }

        [Fact]
        public void AddDays_CenturyLeapYearIncludesFeb29()
        {
            var result = _calculator.AddDays(28, 2, 2000, 1);
            Assert.Equal(new DateModel(29, 2, 2000), result);
        }

        [Fact]
        public void AddDays_RollsOverYearBoundary()
        {
            var result = _calculator.AddDays(31, 12, 2016, 1);
            Assert.Equal(new DateModel(1, 1, 2017), result);
        }

        [Fact]
        public void AddDays_FullLeapYearCycleLandsOnDec31()
        {
            // 2020 is a leap year, 366 days
            var result = _calculator.AddDays(1, 1, 2020, 365);
            Assert.Equal(new DateModel(31, 12, 2020), result);
        }

        [Fact]
        public void AddDays_NegativeDaysSubtractsAcrossLeapBoundary()
        {
            var result = _calculator.AddDays(1, 3, 2016, -1);
            Assert.Equal(new DateModel(29, 2, 2016), result);
        }

        [Fact]
        public void AddDays_NegativeDaysRollsBackAcrossYearBoundary()
        {
            var result = _calculator.AddDays(1, 1, 2017, -1);
            Assert.Equal(new DateModel(31, 12, 2016), result);
        }

        [Fact]
        public void AddDays_LargeAdditionSpansMultipleYears()
        {
            var result = _calculator.AddDays(1, 1, 2020, 1000);
            Assert.Equal(new DateModel(27, 9, 2022), result);
        }

        [Fact]
        public void AddDays_YearDroppingBelowMinimumThrows()
        {
            var exception = Record.Exception(() => _calculator.AddDays(1, 1, 1, -1));
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Fact]
        public void AddDays_YearRisingAbove9999Throws()
        {
            var exception = Record.Exception(() => _calculator.AddDays(31, 12, 9999, 1));
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Fact]
        public void AddDays_InvalidStartingDayThrows()
        {
            // Feb has no 30th
            var exception = Record.Exception(() => _calculator.AddDays(30, 2, 2021, 1));
            Assert.IsType<ArgumentOutOfRangeException>(exception);
        }

        [Fact]
        public void IsLeapYear_OutOfRangeYearLogsErrorAndThrows()
        {
            var exception = Record.Exception(() => _calculator.IsLeapYear(0));

            Assert.IsType<ArgumentOutOfRangeException>(exception);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Invalid year provided")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
