namespace DateAdder
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            string dateInput;
            string daysInput;

            if (args.Length >= 2)
            {
                dateInput = args[0];
                daysInput = args[1];
            }
            else
            {
                Console.Write("Enter starting date (dd/mm/yyyy): ");
                dateInput = Console.ReadLine() ?? string.Empty;
                Console.Write("Enter number of days to add: ");
                daysInput = Console.ReadLine() ?? string.Empty;
            }

            try
            {
                var (day, month, year) = DateParser.Parse(dateInput);

                if (!int.TryParse(daysInput, out int daysToAdd))
                {
                    Console.WriteLine($"Error: '{daysInput}' is not a valid whole number of days.");
                    return 1;
                }

                var (newDay, newMonth, newYear) = DateCalculator.AddDays(day, month, year, daysToAdd);

                Console.WriteLine($"New Date: {DateParser.Format(newDay, newMonth, newYear)}");
                return 0;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }
    }
}
