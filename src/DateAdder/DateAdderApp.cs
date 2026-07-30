using DateAdder.Interfaces;
using Microsoft.Extensions.Logging;

namespace DateAdder
{
    public class DateAdderApp
    {
        private readonly ILogger<DateAdderApp> _logger;
        private readonly IDateCalculationService _dateCalculationService;

        public DateAdderApp(
            ILogger<DateAdderApp> logger,
            IDateCalculationService dateCalculationService)
        {
            _logger = logger;
            _dateCalculationService = dateCalculationService;
        }

        public int Run(string[] args)
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
                if (!int.TryParse(daysInput, out int daysToAdd))
                {
                    _logger.LogError("Invalid days input: {DaysInput}", daysInput);
                    Console.WriteLine($"Error: '{daysInput}' is not a valid whole number of days.");
                    return 1;
                }

                var result = _dateCalculationService.Calculate(dateInput, daysToAdd);
                Console.WriteLine($"New Date: {result.FormattedResult}");
                return 0;
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Format error occurred during date processing.");
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex, "Argument out of range error occurred during date processing.");
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred.");
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return 1;
            }
        }
    }
}
