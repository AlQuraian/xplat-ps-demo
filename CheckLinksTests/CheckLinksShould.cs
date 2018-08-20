using System;
using System.Linq;
using CheckLinksConsole;
using Xunit;

namespace CheckLinksTests
{
    public class CheckLinksShould
    {
        [Fact]
        public void NotParseAnyLink()
        {
            var links = LinkChecker.GetLinks("<a href=\"google.com\">Google</a>");
            links.Count().Equals(0);
        }

        [Fact]
        public void ParseOneLink()
        {
            var url = "http://google.com";
            var links = LinkChecker.GetLinks($"<a href=\"{url}\">Google</a>");
            links.Count().Equals(1);
            Assert.Equal(url, links.First());
        }
    }
}
