namespace DateAdder
{
    public static class DateParser
    {
        public static (int Day, int Month, int Year) Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new FormatException("Date cannot be empty.");

            var parts = input.Split('/');
            if (parts.Length != 3)
                throw new FormatException($"'{input}' is not in dd/mm/yyyy format.");

            // require zero-padded 2/2/4 digits
            if (parts[0].Length != 2 || parts[1].Length != 2 || parts[2].Length != 4)
                throw new FormatException($"'{input}' must use exactly dd/mm/yyyy (2/2/4 digits).");

            // block +/- and spaces slipping through TryParse
            if (!IsDigitsOnly(parts[0]) || !IsDigitsOnly(parts[1]) || !IsDigitsOnly(parts[2]))
                throw new FormatException($"'{input}' must contain only digits 0-9 in each component.");

            if (!int.TryParse(parts[0], out int day) ||
                !int.TryParse(parts[1], out int month) ||
                !int.TryParse(parts[2], out int year))
                throw new FormatException($"'{input}' contains non-numeric components.");

            if (month < 1 || month > 12)
                throw new FormatException($"Month {month} is out of range (1-12).");

            if (year < DateCalculator.MinYear || year > DateCalculator.MaxYear)
                throw new FormatException($"Year {year} is out of range ({DateCalculator.MinYear:D4}-{DateCalculator.MaxYear}).");

            int maxDay = DateCalculator.DaysInMonth(month, year);
            if (day < 1 || day > maxDay)
                throw new FormatException($"Day {day} is out of range for {month:D2}/{year:D4} (max {maxDay}).");

            return (day, month, year);
        }

        public static string Format(int day, int month, int year)
            => $"{day:D2}/{month:D2}/{year:D4}";

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
