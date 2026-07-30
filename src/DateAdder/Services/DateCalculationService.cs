using DateAdder.Interfaces;
using DateAdder.Models;
using Microsoft.Extensions.Logging;

namespace DateAdder.Services;

public sealed class DateCalculationService : IDateCalculationService
{
    private readonly ILogger<DateCalculationService> _logger;
    private readonly IDateParser _dateParser;
    private readonly IDateCalculator _dateCalculator;

    public DateCalculationService(
        ILogger<DateCalculationService> logger,
        IDateParser dateParser,
        IDateCalculator dateCalculator)
    {
        _logger = logger;
        _dateParser = dateParser;
        _dateCalculator = dateCalculator;
    }

    public DateCalculationResult Calculate(string inputDate, int daysToAdd)
    {
        _logger.LogInformation("Processing date calculation for {InputDate} with {DaysToAdd} days.", inputDate, daysToAdd);

        var startDate = _dateParser.Parse(inputDate);
        var resultDate = _dateCalculator.AddDays(startDate.Day, startDate.Month, startDate.Year, daysToAdd);
        var formattedResult = _dateParser.Format(resultDate);

        var calculationResult = new DateCalculationResult(
            new DateModel(startDate.Day, startDate.Month, startDate.Year),
            new DateModel(resultDate.Day, resultDate.Month, resultDate.Year),
            daysToAdd,
            formattedResult);

        _logger.LogInformation("Calculation completed successfully. Result: {Result}", formattedResult);
        return calculationResult;
    }
}
