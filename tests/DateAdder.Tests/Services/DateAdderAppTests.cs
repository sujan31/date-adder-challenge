using DateAdder.Interfaces;
using DateAdder.Models;
using Moq;
using Microsoft.Extensions.Logging;

namespace DateAdder.Tests
{
    public class DateAdderAppTests
    {
        private readonly Mock<ILogger<DateAdderApp>> _loggerMock;
        private readonly Mock<IDateCalculationService> _calculationServiceMock;
        private readonly DateAdderApp _app;

        public DateAdderAppTests()
        {
            _loggerMock = new Mock<ILogger<DateAdderApp>>();
            _calculationServiceMock = new Mock<IDateCalculationService>();
            _app = new DateAdderApp(_loggerMock.Object, _calculationServiceMock.Object);
        }

        [Fact]
        public void Run_ValidArgs_ReturnsSuccess()
        {
            string[] args = { "31/01/2016", "1" };
            _calculationServiceMock
                .Setup(s => s.Calculate("31/01/2016", 1))
                .Returns(new DateCalculationResult(
                    new DateModel(31, 1, 2016),
                    new DateModel(1, 2, 2016),
                    1,
                    "01/02/2016"));

            int result = _app.Run(args);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Run_InvalidDays_ReturnsError()
        {
            string[] args = { "31/01/2016", "abc" };

            int result = _app.Run(args);

            Assert.Equal(1, result);
            _calculationServiceMock.Verify(s => s.Calculate(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Run_ServiceThrows_FormatException_ReturnsError()
        {
            string[] args = { "invalid", "1" };
            _calculationServiceMock.Setup(s => s.Calculate("invalid", 1)).Throws(new FormatException("Invalid format"));

            int result = _app.Run(args);

            Assert.Equal(1, result);
        }

        [Fact]
        public void Run_ServiceThrows_FormatException_LogsError()
        {
            string[] args = { "invalid", "1" };
            _calculationServiceMock.Setup(s => s.Calculate("invalid", 1)).Throws(new FormatException("Invalid format"));

            int result = _app.Run(args);

            Assert.Equal(1, result);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Format error occurred during date processing")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
