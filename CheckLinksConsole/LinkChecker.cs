using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CheckLinksConsole
{
    public class LinkChecker
    {
        public static IEnumerable<string> GetLinks(string page)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(page);
            var links = htmlDocument.DocumentNode.SelectNodes("//a[@href]")
            .Select(n => n.GetAttributeValue("href", string.Empty))
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Where(l => l.StartsWith("http"));

            return links;
        }

        internal static async Task<IEnumerable<LinkCheckResult>> Check(IEnumerable<string> links)
            => await Task.WhenAll(links.Select(CheckLink));

        private static async Task<LinkCheckResult> CheckLink(string link)
        => new LinkCheckResult
        {
            Link = link,
            Problem = await GetProblemIfExists(link)
        };

        private static async Task<string> GetProblemIfExists(string link)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Head, link);

                try
                {
                    var response = await client.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                    {
                        return response.StatusCode.ToString();
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            return string.Empty;
        }
    }

    public class LinkCheckResult
    {
        public bool Exists => string.IsNullOrWhiteSpace(Problem);
        public bool IsMissing => !Exists;
        public string Problem { get; set; }
        public string Link { get; set; }
    }
}