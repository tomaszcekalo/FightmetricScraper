using System;
using System.Collections.Generic;

namespace FightmetricRdsCaScraper
{
    public class Fight
    {
        public string EventName { get; set; }
        public string EventHref { get; set; }
        public List<Performer> Performers { get; set; }
        public string Outcome { get; set; }
        public IEnumerable<string[]> Stats { get; set; }
        public String StrikesRedBodyPercentage { get; set; }
        public String StrikesRedHeadPercentage { get; set; }
        public String StrikesRedLegPercentage { get; set; }
        public String StrikesBlueBodyPercentage { get; set; }
        public string StrikesBlueHeadPercentage { get; set; }
        public string StrikesBlueLegPercentage { get; set; }
        public Dictionary<string, string[]> LandedByRound { get; set; }
    }
}