using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
            var site = configuration["site"];
            var body = await new HttpClient().GetStringAsync(site);

            System.Console.WriteLine();
            System.Console.WriteLine("Links");
            var links = LinkChecker.GetLinks(body);
            links.ToList().ForEach(System.Console.WriteLine);

            var output = configuration.GetSection("output").Get<OutputSettings>();
            Directory.CreateDirectory(output.ReportDirectory);

            var checkedLinks = await LinkChecker.Check(links);
            using (var file = File.CreateText(output.ReportFilePath))
            {
                foreach (var link in checkedLinks.OrderBy(l => l.Exists))
                {
                    var status = link.IsMissing ? "Missing" : "Ok";
                    await file.WriteLineAsync($"{status} - {link.Link}");
                }
            }
        }

        private static IConfigurationRoot GetConfiguration(string[] args)
        {
            var inMemory = new Dictionary<string, string>
            {
                {"site", "https://google.com"}
            };

            var configBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("checksettings.json", true)
                .AddCommandLine(args)
                .AddEnvironmentVariables();
            return configBuilder.Build();
        }
    }
}
