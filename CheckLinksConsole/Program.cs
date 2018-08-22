using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CheckLinksConsole
{
    class OutputSettings
    {
        public string Folder { get; set; }
        public string File { get; set; }
        public string ReportFilePath => Path.Combine(Directory.GetCurrentDirectory(), Folder, File);
        public string ReportDirectory => Path.GetDirectoryName(ReportFilePath);
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = GetConfiguration(args);
            var links = await LinkChecker.GetLinks(configuration["site"]);
            var output = configuration.GetSection("output").Get<OutputSettings>();
            Directory.CreateDirectory(output.ReportDirectory);

            Logs.Init(configuration);
            var logger = Logs.Factory.CreateLogger<Program>();
            logger.LogInformation($"Saving report to {output.ReportFilePath}");

            var checkedLinks = await LinkChecker.Check(links);
            using (var file = File.CreateText(output.ReportFilePath))
            using (var linksDb = new LinksDb())
            {
                foreach (var link in checkedLinks.OrderBy(l => l.Exists))
                {
                    var status = link.IsMissing ? "Missing" : "Ok";
                    await file.WriteLineAsync($"{status} - {link.Link}");
                    await linksDb.AddAsync(link);
                }
                await linksDb.SaveChangesAsync();
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
