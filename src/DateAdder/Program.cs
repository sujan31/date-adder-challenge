using DateAdder.Interfaces;
using DateAdder.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DateAdder
{
    public class Program
    {
        public static int Main(string[] args)
        {
            using var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                })
                .AddSingleton<IDateCalculator, DateCalculator>()
                .AddSingleton<IDateParser, DateParser>()
                .AddSingleton<IDateCalculationService, DateCalculationService>()
                .AddSingleton<DateAdderApp>()
                .BuildServiceProvider();

            var app = serviceProvider.GetRequiredService<DateAdderApp>();
            return app.Run(args);
        }
    }
}
