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
            var result = new FightmetricEvent
            {
                Title = node.CssSelect(".b-container__event_details .b-title")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim(),
                Date = node.CssSelect(".b-title__text")
                    .LastOrDefault()
                    ?.InnerText
                    .Trim(),
                Location = node.CssSelect("p.b-title__text")
                    .Select(x => x.InnerText.Trim())
                    .Take(2)
                    .ToArray(),
                Fights = node.CssSelect("tbody.b-table__body tr.b-table__row")
                    .Select(ParseEventFight)
                    .ToList()
            };
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
            var redDonut = node.CssSelect("#b-container__red_fighter_donut")
                .FirstOrDefault();
            var blueDonut = node.CssSelect("#b-container__blue_fighter_donut")
                .FirstOrDefault();
            var result = new Fight
            {
                EventName = node.CssSelect(".b-container__fighter_header_dark")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim(),
                EventHref = node.CssSelect(".b-container__fighter_header_dark a")
                    .FirstOrDefault()
                    ?.Attributes["href"]
                    .Value,
                Performers = node.CssSelect(".b-container__breakdown_fighter_name")
                    .Select(ParsePerformer)
                    .ToList(),
                Outcome = node.CssSelect(".b-container__result_outcome_description")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim(),
                Stats = node.CssSelect(".b-container__result_fighter_info")
                    .CssSelect("div")
                    .Select(x => x.CssSelect("div")
                        .Select(y => y.InnerText).ToArray()),
                StrikesRedBodyPercentage = redDonut
                    ?.Attributes["data_body"]
                    .Value,
                StrikesRedHeadPercentage = redDonut
                    ?.Attributes["data_head"]
                    .Value,
                StrikesRedLegPercentage = redDonut
                    ?.Attributes["data_leg"]
                    .Value,
                StrikesBlueBodyPercentage = blueDonut
                    ?.Attributes["data_body"]
                    .Value,
                StrikesBlueHeadPercentage = blueDonut
                    ?.Attributes["data_head"]
                    .Value,
                StrikesBlueLegPercentage = blueDonut
                    ?.Attributes["data_leg"]
                    .Value,
                LandedByRound = node
                    .CssSelect("div.b-container__results_bar_graphs div.b-container__bar_couple")
                    .Select(x => new
                    {
                        Round = x.CssSelect(".b-container__bar_couple_label")
                            .FirstOrDefault()
                            ?.InnerText
                            .Trim(),
                        Numbers = x.CssSelect("div.b-container__bar_couple_bar")
                            .Select(y => y.Attributes["data_value"].Value)
                            .ToArray()
                    })
                    .ToDictionary(x => x.Round, x => x.Numbers)
            };
            return result;
        }

        public Performer ParsePerformer(HtmlNode node)
        {
            var link = node.CssSelect("a")
                .FirstOrDefault();
            Performer result = new Performer
            {
                Href = link
                    ?.Attributes["href"]
                    .Value,
                Name = link?
                    .InnerText
                    .Trim(),
                Nick = node.CssSelect("h4")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim(),
                Score = node.CssSelect(".b-container_tight_record span")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim()
            };
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
            var result = new Fighter
            {
                Fights = node
                    .CssSelect(".b-container__fighter_fights tbody tr")
                    .Select(ParseFighterFights)
                    .ToList(),
                Weight = attributes["Weight"]
                    ?.InnerText
                    .Trim(),
                Height = attributes["Height"]
                    ?.InnerText
                    .Trim(),
                Reach = attributes["Reach"]
                    ?.InnerText
                    .Trim(),
                Age = attributes["Age"]
                    ?.InnerText
                    .Trim(),
                Stance = attributes["Stance"]
                    ?.InnerText
                    .Trim(),
                Born = attributes["Born"]
                    ?.InnerText
                    .Trim(),
                FightsOutOf = attributes["Fights Out Of"]
                    ?.InnerText
                    .Trim(),
                StrikesLandedPerMinute = node
                    .CssSelect("#slpm")
                    .FirstOrDefault()
                    ?.Attributes["data_value"]
                    .Value,
                StrikesAbsorbedPerMinute = node
                    .CssSelect("#sapm")
                    .FirstOrDefault()
                    ?.Attributes["data_value"]
                    .Value,
                StrikesUFCAvg = node
                    .CssSelect("#st-ufc-avg")
                    .FirstOrDefault()
                    ?.Attributes["data_value"]
                    .Value,
                TakedownsLandedPer15 = node
                    .CssSelect("#td-per-15")
                    .FirstOrDefault()
                    ?.Attributes["data_value"]
                    .Value,
                TakedownsLandedUFCAvg = node
                    .CssSelect("#td-ufc-avg")
                    .FirstOrDefault()
                    ?.Attributes["data_value"]
                    .Value,
                SubmissionAttemptsPer15 = node
                    .CssSelect("#sb-per-15")
                    .FirstOrDefault()
                    ?.Attributes["data_value"]
                    .Value,
                SubmissionAttemptsUFCAvg = node
                    .CssSelect("#sb-ufc-avg")
                    .FirstOrDefault()
                    ?.Attributes["data_value"]
                    .Value,
                StrikingPercentage = offensiveBreakdownGraph
                    ?.Attributes["data_striking_percentage"]
                    .Value,
                SubmissionsPercentage = offensiveBreakdownGraph
                    ?.Attributes["data_submission_percentage"]
                    .Value,
                TakedownsPercentage = offensiveBreakdownGraph
                    ?.Attributes["data_takedown_percentage"]
                    .Value,
                StrikingAccuracy = pieGraphs
                    ?.Attributes["data_striking_accuracy"]
                    .Value,
                StrikingDefence = pieGraphs
                    ?.Attributes["data_striking_defence"]
                    .Value,
                TakedownAccuracy = pieGraphs
                    ?.Attributes["data_takedown_accuracy"]
                    .Value,
                TakedownDefence = pieGraphs
                    ?.Attributes["data_takedown_defence"]
                    .Value
            };

            return result;
        }

        private FighterFight ParseFighterFights(HtmlNode node)
        {
            var cells = node.CssSelect("td")
                .ToList();
            var result = new FighterFight
            {
                WL = node.CssSelect(".b-container__fight_outcome_highlight")
                    .FirstOrDefault()
                    ?.InnerText,
                Weight = cells[1].InnerText.Trim(),
                Belt = cells[1].CssSelect("img").Any(),
                Fighters = cells[2].CssSelect("a")
                    .ToDictionary(x => x.InnerText.Trim(),
                        x => x.Attributes["href"].Value),
                KD = cells[3].InnerText.Trim(),
                STR = cells[4].InnerText.Trim(),
                TD = cells[5].InnerText.Trim(),
                SUB = cells[6].InnerText.Trim(),
                EventName = cells[7].CssSelect("a")
                    .FirstOrDefault()
                    ?.InnerText
                    .Trim(),
                EventHref = cells[7].CssSelect("a")
                    .FirstOrDefault()
                    ?.Attributes["href"]
                    .Value,
                Method = cells[8]
                    .InnerText.Trim(),
                Round = cells[9]
                    .InnerText.Trim(),
                Time = cells[10]
                    .InnerText.Trim(),
                StatsHref = cells[11]
                    .CssSelect("a")
                    .FirstOrDefault()
                    ?.Attributes["href"]
                    .Value
            };
            return result;
        }
    }
}