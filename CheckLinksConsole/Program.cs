using System;
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
            var client = new HttpClient();
            var body = await client.GetStringAsync(site);
            System.Console.WriteLine(body);

            System.Console.WriteLine();
            System.Console.WriteLine("Links");
            var links = LinkChecker.GetLinks(body);
            links.ToList().ForEach(System.Console.WriteLine);
        }
    }
}
