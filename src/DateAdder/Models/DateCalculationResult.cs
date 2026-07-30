namespace DateAdder.Models;

public sealed record DateCalculationResult(
    DateModel StartDate,
    DateModel ResultDate,
    int DaysAdded,
    string FormattedResult);
