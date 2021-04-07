using Newtonsoft.Json;
using System;

namespace FightmetricRdsCaScraper.ConsoleExample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var scraper = new FightmetricScraper();
            var fighter = scraper.ScrapeFighter("https://fightmetric.rds.ca/fighter/2300");
            Console.WriteLine(JsonConvert.SerializeObject(fighter, Formatting.Indented));
            var result = scraper
                .ScrapeEvents(
                    Consts.CompletedUrl);
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            foreach (var item in result)
            {
                var fmevent = scraper.ScrapeEvent(Consts.BaseUrl + item.EventHref);
                Console.WriteLine(JsonConvert.SerializeObject(fmevent, Formatting.Indented));
            }
            result = scraper
                .ScrapeEvents(
                    Consts.UpcomingUrl);
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Console.ReadKey();
        }
    }
}