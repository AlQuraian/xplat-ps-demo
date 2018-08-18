using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace CheckLinksConsole
{
    internal class LinkChecker
    {
        internal static IEnumerable<string> GetLinks(string page)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(page);
            var links = htmlDocument.DocumentNode.SelectNodes("//a[@href]")
            .Select(n => n.GetAttributeValue("href", string.Empty))
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Where(l => l.StartsWith("http"));

            return links;
        }
    }
}