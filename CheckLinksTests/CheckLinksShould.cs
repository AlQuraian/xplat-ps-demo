using System;
using System.Linq;
using System.Threading.Tasks;
using CheckLinksConsole;
using Xunit;

namespace CheckLinksTests
{
    public class CheckLinksShould
    {
        [Fact]
        public async Task ExtractedALink()
        {
            var links = await LinkChecker.GetLinks("https://google.com");
            Assert.True(links.Count() > 0);
        }
    }
}
