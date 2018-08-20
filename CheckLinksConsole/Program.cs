using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CheckLinksConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var site = "https://g0t4.github.io/pluralsight-dotnet-core-xplat-apps";
            var body = await new HttpClient().GetStringAsync(site);
            System.Console.WriteLine(body);

            System.Console.WriteLine();
            System.Console.WriteLine("Links");
            var links = LinkChecker.GetLinks(body);
            links.ToList().ForEach(System.Console.WriteLine);

            var currentDirectory = Directory.GetCurrentDirectory();
            var outputFolder = "reports";
            var outputFile = "report.txt";
            var outputPath = Path.Combine(currentDirectory, outputFolder, outputFile);
            var directory = Path.GetDirectoryName(outputPath);
            Directory.CreateDirectory(directory);
            // File.WriteAllLines(outputPath, links);

            var checkedLinks = await LinkChecker.Check(links);
            using (var file = File.CreateText(outputPath))
            {
                foreach (var link in checkedLinks.OrderBy(l => l.Exists))
                {
                    var status = link.IsMissing ? "Missing" : "Ok";
                    await file.WriteLineAsync($"{status} - {link.Link}");
                }
            }
        }
    }
}
