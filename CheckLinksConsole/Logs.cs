using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CheckLinksConsole
{
    public static class Logs
    {
        public static LoggerFactory Factory = new LoggerFactory();

        public static void Init(IConfiguration configuration)
        {
            // Factory.AddConsole(minLevel: LogLevel.Trace, includeScopes: true);
            Factory.AddConsole(configuration.GetSection("Logging"));
            Factory.AddFile("logs/checklinks-{Date}.txt", minimumLevel: LogLevel.Trace);
        }
    }
}