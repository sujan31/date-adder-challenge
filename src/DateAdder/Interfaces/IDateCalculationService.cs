using DateAdder.Models;

namespace DateAdder.Interfaces;

public interface IDateCalculationService
{
    DateCalculationResult Calculate(string inputDate, int daysToAdd);
}
