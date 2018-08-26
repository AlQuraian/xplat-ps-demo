using System;
using System.Collections.Generic;
using System.IO;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;

namespace CheckLinksConsole
{
    public class OutputSettings
    {
        public string Folder { get; set; }
        public string File { get; set; }
        public string ReportFilePath => Path.Combine(Directory.GetCurrentDirectory(), Folder, File);
        public string ReportDirectory => Path.GetDirectoryName(ReportFilePath);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var appConfig = GetConfiguration(args);
            Logs.Init(appConfig);

            GlobalConfiguration.Configuration.UseMemoryStorage();

            var host = WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
            var settings = appConfig.GetSection("output").Get<OutputSettings>();
            var site = appConfig["site"];

            RecurringJob.AddOrUpdate<CheckLinkJob>("check-links", j => j.Execute(site, settings), Cron.Minutely);
            RecurringJob.Trigger("check-links");

            host.Build().Run();
        }

        private static IConfigurationRoot GetConfiguration(string[] args)
        {
            var inMemory = new Dictionary<string, string>
            {
                {"site", "https://google.com"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("checksettings.json", optional: false)
                .AddCommandLine(args)
                .AddEnvironmentVariables().Build();
        }
    }
}
