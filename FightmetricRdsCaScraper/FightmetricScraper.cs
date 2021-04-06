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
        Fight ScrapeFight(string url);
        Fighter ScrapeFighter(string url);
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
                .LastOrDefault()
                ?.InnerText
                .Trim();
            result.Location = node.CssSelect("p.b-title__text")
                .Select(x => x.InnerText.Trim())
                .Take(2)
                .ToArray();
            result.Fights = node.CssSelect("tbody.b-table__body tr.b-table__row")
                .Select(ParseEventFight)
                .ToList();
            return result;
        }

        public EventFight ParseEventFight(HtmlNode node)
        {
            var cells = node.CssSelect("td.b-table__col")
                .ToList();
            var result = new EventFight
            {
                Fighters = cells[1]
                    .CssSelect("div")
                    .Select(ParseEventFighter)
                    .ToList(),
                Weight = cells[2]
                    .CssSelect("div")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim(),
                Belt = cells[2]
                    .CssSelect("div.b-table__img img")
                    .Any(x => x.Attributes["alt"].Value == "belt"),
                KnockDowns = cells[3]
                    .CssSelect("div")
                    .Select(x => x.InnerText)
                    .ToList(),
                Strikes = cells[4]
                    .CssSelect("div")
                    .Select(x => x.InnerText)
                    .ToList(),
                Takedowns = cells[5]
                    .CssSelect("div")
                    .Select(x => x.InnerText)
                    .ToList(),
                SubmissionAttempts = cells[6]
                    .CssSelect("div")
                    .Select(x => x.InnerText)
                    .ToList(),
                Method = cells[7]
                    .CssSelect("div")
                    .Select(x => x.InnerText.Trim())
                    .ToList(),
                Round = cells[8].InnerText.Trim(),
                Time = cells[9].InnerText.Trim(),
                StatsHref = cells[10]
                    .CssSelect("a")
                    .FirstOrDefault()
                    ?.Attributes["href"]
                    .Value
            };
            return result;
        }

        public EventFighter ParseEventFighter(HtmlNode node)
        {
            var result = new EventFighter
            {
                Name = node.CssSelect("a.b-table__link")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim(),
                Href = node.CssSelect("a.b-table__link")
                    .FirstOrDefault()
                    ?.Attributes["href"]
                    .Value,
                Winner = node.CssSelect("span.b-container__event_detail_winner_label")
                    .Any()
            };
            return result;
        }
        public Fight ScrapeFight(string url)
        {
            WebPage homePage = _browser.NavigateToPage(new Uri(url));
            return ParseFight(homePage.Html);
        }

        public Fight ParseFight(HtmlNode node)
        {
            var result = new Fight();
            return result;
        }

        public Fighter ScrapeFighter(string url)
        {
            WebPage homePage = _browser.NavigateToPage(new Uri(url));
            return ParseFighter(homePage.Html);
        }
        public Fighter ParseFighter(HtmlNode node)
        {
            var result = new Fighter();
            return result;
        }
    }
}