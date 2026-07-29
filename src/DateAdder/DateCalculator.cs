namespace DateAdder
{
    public static class DateCalculator
    {
        private static readonly int[] DaysInMonthNonLeap =
            { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        internal const int MinYear = 1;
        internal const int MaxYear = 9999;

        public static bool IsLeapYear(int year)
        {
            if (year < MinYear || year > MaxYear)
                throw new ArgumentOutOfRangeException(nameof(year), $"Year must be {MinYear}-{MaxYear}, was {year}.");

            if (year % 4 != 0) return false;
            if (year % 100 != 0) return true;
            return year % 400 == 0;
        }

        public static int DaysInMonth(int month, int year)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month), $"Month must be 1-12, was {month}.");
            if (year < MinYear || year > MaxYear)
                throw new ArgumentOutOfRangeException(nameof(year), $"Year must be {MinYear}-{MaxYear}, was {year}.");

            return month == 2 && IsLeapYear(year) ? 29 : DaysInMonthNonLeap[month - 1];
        }

        public static (int Day, int Month, int Year) AddDays(int day, int month, int year, int daysToAdd)
        {
            int maxStartDay = DaysInMonth(month, year);
            if (day < 1 || day > maxStartDay)
                throw new ArgumentOutOfRangeException(nameof(day), $"Day must be 1-{maxStartDay} for {month:D2}/{year:D4}, was {day}.");

            int d = day, m = month, y = year;

            // avoid overflow on int.MinValue
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

                // check year range
                if (y < MinYear || y > MaxYear)
                    throw new ArgumentOutOfRangeException(nameof(daysToAdd),
                        $"Result year {y} is outside the supported {MinYear:D4}-{MaxYear} range.");
            }

            return (d, m, y);
        }
    }
}
