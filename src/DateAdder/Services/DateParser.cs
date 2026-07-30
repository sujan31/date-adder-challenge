using DateAdder.Interfaces;
using DateAdder.Models;
using Microsoft.Extensions.Logging;

namespace DateAdder
{
    public class DateParser : IDateParser
    {
        private readonly ILogger<DateParser> _logger;
        private readonly IDateCalculator _dateCalculator;

        public DateParser(ILogger<DateParser> logger, IDateCalculator dateCalculator)
        {
            _logger = logger;
            _dateCalculator = dateCalculator;
        }

        public DateModel Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                _logger.LogError("Date input is empty.");
                throw new FormatException("Date cannot be empty.");
            }

            var parts = input.Split('/');
            if (parts.Length != 3)
            {
                _logger.LogError("Invalid date format: {Input}. Expected dd/mm/yyyy.", input);
                throw new FormatException($"'{input}' is not in dd/mm/yyyy format.");
            }

            if (parts[0].Length != 2 || parts[1].Length != 2 || parts[2].Length != 4)
            {
                _logger.LogError("Invalid component lengths in date: {Input}. Expected 2/2/4 digits.", input);
                throw new FormatException($"'{input}' must use exactly dd/mm/yyyy (2/2/4 digits).");
            }

            if (!IsDigitsOnly(parts[0]) || !IsDigitsOnly(parts[1]) || !IsDigitsOnly(parts[2]))
            {
                _logger.LogError("Date components contain non-digit characters: {Input}.", input);
                throw new FormatException($"'{input}' must contain only digits 0-9 in each component.");
            }

            if (!int.TryParse(parts[0], out int day) ||
                !int.TryParse(parts[1], out int month) ||
                !int.TryParse(parts[2], out int year))
            {
                _logger.LogError("Failed to parse date components to integers: {Input}.", input);
                throw new FormatException($"'{input}' contains non-numeric components.");
            }

            if (month < 1 || month > 12)
            {
                _logger.LogError("Month out of range: {Month}.", month);
                throw new FormatException($"Month {month} is out of range (1-12).");
            }

            if (year < DateCalculator.MinYear || year > DateCalculator.MaxYear)
            {
                _logger.LogError("Year out of range: {Year}. Supported range is {MinYear}-{MaxYear}.", year, DateCalculator.MinYear, DateCalculator.MaxYear);
                throw new FormatException($"Year {year} is out of range ({DateCalculator.MinYear:D4}-{DateCalculator.MaxYear}).");
            }

            int maxDay = _dateCalculator.DaysInMonth(month, year);
            if (day < 1 || day > maxDay)
            {
                _logger.LogError("Day out of range: {Day} for {Month:D2}/{Year:D4}. Max day is {MaxDay}.", day, month, year, maxDay);
                throw new FormatException($"Day {day} is out of range for {month:D2}/{year:D4} (max {maxDay}).");
            }

            return new DateModel(day, month, year);
        }

        public string Format(DateModel date)
            => $"{date.Day:D2}/{date.Month:D2}/{date.Year:D4}";

        private static bool IsDigitsOnly(string value)
        {
            foreach (char c in value)
            {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }
    }
}
