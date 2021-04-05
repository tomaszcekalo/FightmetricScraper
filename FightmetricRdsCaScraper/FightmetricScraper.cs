using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FightmetricRdsCaScraper
{
    public interface IFightmetricScraper
    {
        List<EventsItem> ScrapeEvents(string url);
        FightmetricEvent ScrapeEvent(string url);
    }

    public class FightmetricScraper : IFightmetricScraper
    {
        private readonly ScrapingBrowser _browser;
        public FightmetricScraper()
        {
            _browser = new ScrapingBrowser();
        }

        public FightmetricScraper(ScrapingBrowser browser)
        {
            _browser = browser;
        }

        public List<EventsItem> ScrapeEvents(string url)
        {
            WebPage homePage = _browser.NavigateToPage(new Uri(url));
            return ParseEventItems(homePage.Html);
        }

        public List<EventsItem> ParseEventItems(HtmlNode node)
        {
            var result = node
                .CssSelect("table.b-table tbody.b-table__body tr.b-table__row")
                .Select(ParseEventsItem)
                .ToList();
            return result;
        }

        public EventsItem ParseEventsItem(HtmlNode node)
        {
            var a = node.CssSelect("a.b-table__link")
                .FirstOrDefault();
            var cells = node.CssSelect("td");
            var result = new EventsItem
            {
                Location = cells
                    .LastOrDefault()
                    ?.InnerText
                    .Trim(),
                Date = cells
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim(),
                EventName = a?.InnerText.Trim(),
                EventHref = a?.Attributes["href"].Value
            };
            return result;
        }
        public FightmetricEvent ScrapeEvent(string url)
        {
            WebPage homePage = _browser.NavigateToPage(new Uri(url));
            return ParseFightmetricEvent(homePage.Html);
        }

        public FightmetricEvent ParseFightmetricEvent(HtmlNode node)
        {
            var result = new FightmetricEvent();
            result.Title = node.CssSelect(".b-container__event_details .b-title")
                .FirstOrDefault()
                ?.InnerText
                .Trim();
            result.Date = node.CssSelect(".b-title__text")
                .FirstOrDefault()
                ?.InnerText
                .Trim();
            result.Location = node.CssSelect("p.b-title__text")
                .Select(x => x.InnerText.Trim())
                .Take(2)
                .ToArray();

            return result;
        }
    }
}