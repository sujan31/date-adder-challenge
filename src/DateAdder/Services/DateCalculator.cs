using DateAdder.Interfaces;
using DateAdder.Models;
using Microsoft.Extensions.Logging;

namespace DateAdder
{
    public class DateCalculator : IDateCalculator
    {
        private readonly ILogger<DateCalculator> _logger;

        public DateCalculator(ILogger<DateCalculator> logger)
        {
            _logger = logger;
        }

        private static readonly int[] DaysInMonthNonLeap =
            { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        public const int MinYear = 1;
        public const int MaxYear = 9999;

        public bool IsLeapYear(int year)
        {
            if (year < MinYear || year > MaxYear)
            {
                _logger.LogError("Invalid year provided: {Year}. Year must be between {MinYear} and {MaxYear}.", year, MinYear, MaxYear);
                throw new ArgumentOutOfRangeException(nameof(year), $"Year must be {MinYear}-{MaxYear}, was {year}.");
            }

            if (year % 4 != 0) return false;
            if (year % 100 != 0) return true;
            return year % 400 == 0;
        }

        public int DaysInMonth(int month, int year)
        {
            if (month < 1 || month > 12)
            {
                _logger.LogError("Invalid month provided: {Month}. Month must be between 1 and 12.", month);
                throw new ArgumentOutOfRangeException(nameof(month), $"Month must be 1-12, was {month}.");
            }
            if (year < MinYear || year > MaxYear)
            {
                _logger.LogError("Invalid year provided: {Year}. Year must be between {MinYear} and {MaxYear}.", year, MinYear, MaxYear);
                throw new ArgumentOutOfRangeException(nameof(year), $"Year must be {MinYear}-{MaxYear}, was {year}.");
            }

            return month == 2 && IsLeapYear(year) ? 29 : DaysInMonthNonLeap[month - 1];
        }

        public DateModel AddDays(int day, int month, int year, int daysToAdd)
        {
            _logger.LogInformation("Adding {DaysToAdd} days to {Day:D2}/{Month:D2}/{Year:D4}", daysToAdd, day, month, year);

            int maxStartDay = DaysInMonth(month, year);
            if (day < 1 || day > maxStartDay)
            {
                _logger.LogError("Invalid day provided: {Day} for {Month:D2}/{Year:D4}. Max day is {MaxDay}.", day, month, year, maxStartDay);
                throw new ArgumentOutOfRangeException(nameof(day), $"Day must be 1-{maxStartDay} for {month:D2}/{year:D4}, was {day}.");
            }

            int d = day, m = month, y = year;

            long remaining = Math.Abs((long)daysToAdd);
            bool forward = daysToAdd >= 0;

            for (long i = 0; i < remaining; i++)
            {
                if (forward)
                {
                    d++;
                    if (d > DaysInMonth(m, y))
                    {
                        d = 1;
                        m++;
                        if (m > 12)
                        {
                            m = 1;
                            y++;
                        }
                    }
                }
                else
                {
                    d--;
                    if (d < 1)
                    {
                        m--;
                        if (m < 1)
                        {
                            m = 12;
                            y--;
                        }
                        d = DaysInMonth(m, y);
                    }
                }

                if (y < MinYear || y > MaxYear)
                {
                    _logger.LogError("Resulting year {Year} is outside the supported range {MinYear}-{MaxYear}.", y, MinYear, MaxYear);
                    throw new ArgumentOutOfRangeException(nameof(daysToAdd),
                        $"Result year {y} is outside the supported {MinYear:D4}-{MaxYear} range.");
                }
            }

            _logger.LogInformation("Resulting date: {Day:D2}/{Month:D2}/{Year:D4}", d, m, y);
            return new DateModel(d, m, y);
        }
    }
}
