using DateAdder.Interfaces;
using DateAdder.Models;
using Moq;
using Microsoft.Extensions.Logging;

namespace DateAdder.Tests
{
    public class DateParserTests
    {
        private readonly Mock<ILogger<DateParser>> _loggerMock;
        private readonly Mock<IDateCalculator> _calculatorMock;
        private readonly DateParser _parser;

        public DateParserTests()
        {
            _loggerMock = new Mock<ILogger<DateParser>>();
            _calculatorMock = new Mock<IDateCalculator>();
            _parser = new DateParser(_loggerMock.Object, _calculatorMock.Object);
        }

        [Fact]
        public void Parse_ValidDate_ReturnsComponents()
        {
            _calculatorMock.Setup(c => c.DaysInMonth(1, 2016)).Returns(31);
            var result = _parser.Parse("31/01/2016");
            Assert.Equal(31, result.Day);
            Assert.Equal(1, result.Month);
            Assert.Equal(2016, result.Year);
        }

        [Theory]
        [InlineData("2016/01/31")]  // wrong order
        [InlineData("31-01-2016")]  // wrong separator
        [InlineData("1/1/2016")]    // not zero-padded
        [InlineData("31/13/2016")]
        [InlineData("31/02/2020")]  // Feb has no 31st
        [InlineData("29/02/2019")]  // not a leap year
        [InlineData("00/01/2020")]
        [InlineData("ab/01/2020")]
        [InlineData("")]
        [InlineData("+1/02/2020")]  // sign character
        [InlineData("01/+2/2020")]
        [InlineData("01/01/+202")]
        [InlineData("01/01/-001")]  // negative year
        [InlineData(" 1/02/2020")]  // whitespace
        [InlineData("1 /02/2020")]
        [InlineData("01/01/0000")]  // year 0 not allowed, range starts at 0001
        public void Parse_InvalidInput_ThrowsFormatException(string input)
        {
            _calculatorMock.Setup(c => c.DaysInMonth(It.IsAny<int>(), It.IsAny<int>())).Returns(28); // default for invalid tests
            Assert.Throws<FormatException>(() => _parser.Parse(input));
        }

        [Fact]
        public void Format_PadsSingleDigitComponents()
        {
            Assert.Equal("01/02/2016", _parser.Format(new DateModel(1, 2, 2016)));
        }

        [Fact]
        public void Parse_EmptyInput_LogsErrorAndThrows()
        {
            var exception = Record.Exception(() => _parser.Parse(string.Empty));

            Assert.IsType<FormatException>(exception);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Date input is empty")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
