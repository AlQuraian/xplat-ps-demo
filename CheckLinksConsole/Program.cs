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

            RecurringJob.AddOrUpdate<CheckLinkJob>("check-links", j => j.Execute(appConfig["site"], appConfig.GetSection("output").Get<OutputSettings>()), Cron.Minutely);
            RecurringJob.Trigger("check-links");

            var host = WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

            using (var server = new BackgroundJobServer())
            {
                // System.Console.WriteLine("Hangfire Server started. Press any key to exit...");
                // Console.ReadKey();

                System.Console.WriteLine("Hangfire Server started.");
                host.Build().Run();
            }
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
