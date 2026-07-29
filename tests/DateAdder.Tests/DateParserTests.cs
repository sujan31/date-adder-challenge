namespace DateAdder.Tests
{
    public class DateParserTests
    {
        [Fact]
        public void Parse_ValidDate_ReturnsComponents()
        {
            var (day, month, year) = DateParser.Parse("31/01/2016");
            Assert.Equal(31, day);
            Assert.Equal(1, month);
            Assert.Equal(2016, year);
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
            Assert.Throws<FormatException>(() => DateParser.Parse(input));
        }

        [Fact]
        public void Format_PadsSingleDigitComponents()
        {
            Assert.Equal("01/02/2016", DateParser.Format(1, 2, 2016));
        }
    }
}
