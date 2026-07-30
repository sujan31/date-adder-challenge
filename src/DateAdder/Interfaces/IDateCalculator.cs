using DateAdder.Models;

namespace DateAdder.Interfaces
{
    public interface IDateCalculator
    {
        bool IsLeapYear(int year);
        int DaysInMonth(int month, int year);
        DateModel AddDays(int day, int month, int year, int daysToAdd);
    }
}
