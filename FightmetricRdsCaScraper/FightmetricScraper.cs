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
            var attributes = node
                .CssSelect("div.b-container__fighter_attributes li.b-box__item")
                .Select(x => new
                {
                    label = x.CssSelect("i.b-box__label")
                        .FirstOrDefault()
                        ?.InnerText
                        .Trim(),
                    value = x.CssSelect("i.b-box__value")
                        .FirstOrDefault()
                })
                .Where(x => x.label != null)
                .ToDictionary(
                    x => x.label,
                    x => x.value);

            var pieGraphs = node.CssSelect("#pie-graphs")
                .FirstOrDefault();
            var offensiveBreakdownGraph = node
                .CssSelect("#offensive-breakdown-graph")
                .FirstOrDefault();
            var result = new Fighter();
            result.Weight = attributes["Weight"]
                ?.InnerText
                .Trim();
            result.Height = attributes["Height"]
                ?.InnerText
                .Trim();
            result.Reach = attributes["Reach"]
                ?.InnerText
                .Trim();
            result.Age = attributes["Age"]
                ?.InnerText
                .Trim();
            result.Stance = attributes["Stance"]
                ?.InnerText
                .Trim();
            result.Born = attributes["Born"]
                ?.InnerText
                .Trim();
            result.FightsOutOf = attributes["Fights Out Of"]
                ?.InnerText
                .Trim();
            result.StrikesLandedPerMinute = node
                .CssSelect("#slpm")
                .FirstOrDefault()
                ?.Attributes["data_value"]
                .Value;
            result.StrikesAbsorbedPerMinute = node
                .CssSelect("#sapm")
                .FirstOrDefault()
                ?.Attributes["data_value"]
                .Value;
            result.StrikesUFCAvg = node
                .CssSelect("#st-ufc-avg")
                .FirstOrDefault()
                ?.Attributes["data_value"]
                .Value;
            result.TakedownsLandedPer15 = node
                .CssSelect("#td-per-15")
                .FirstOrDefault()
                ?.Attributes["data_value"]
                .Value;
            result.TakedownsLandedUFCAvg = node
                .CssSelect("#td-ufc-avg")
                .FirstOrDefault()
                ?.Attributes["data_value"]
                .Value;
            result.SubmissionAttemptsPer15 = node
                .CssSelect("#sb-per-15")
                .FirstOrDefault()
                ?.Attributes["data_value"]
                .Value;
            result.SubmissionAttemptsUFCAvg = node
                .CssSelect("#sb-ufc-avg")
                .FirstOrDefault()
                ?.Attributes["data_value"]
                .Value;
            result.StrikingPercentage = offensiveBreakdownGraph
                ?.Attributes["data_striking_percentage"]
                .Value;
            result.SubmissionsPercentage = offensiveBreakdownGraph
                ?.Attributes["data_submission_percentage"]
                .Value;
            result.TakedownsPercentage = offensiveBreakdownGraph
                ?.Attributes["data_takedown_percentage"]
                .Value;
            result.StrikingAccuracy = pieGraphs
                ?.Attributes["data_striking_accuracy"]
                .Value;
            result.StrikingDefence = pieGraphs
                ?.Attributes["data_striking_defence"]
                .Value;
            result.TakedownAccuracy = pieGraphs
                ?.Attributes["data_takedown_accuracy"]
                .Value;
            result.TakedownDefence = pieGraphs
                ?.Attributes["data_takedown_defence"]
                .Value;
            return result;
        }
    }
}