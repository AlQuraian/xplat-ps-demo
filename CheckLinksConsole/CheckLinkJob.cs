using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CheckLinksConsole
{
    public class CheckLinkJob
    {
        public async Task Execute(string site, OutputSettings output)
        {
            // var output = appConfig.GetSection("output").Get<OutputSettings>();
            Directory.CreateDirectory(output.ReportDirectory);

            var logger = Logs.Factory.CreateLogger<CheckLinkJob>();
            logger.LogInformation($"Saving report to {output.ReportFilePath}");

            var links = await LinkChecker.GetLinks(site);
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
    }
}

