using DateAdder.Models;

namespace DateAdder.Interfaces
{
    public interface IDateParser
    {
        DateModel Parse(string input);
        string Format(DateModel date);
    }
}
