using System.Collections.Generic;

namespace FightmetricRdsCaScraper
{
    public class FightmetricEvent
    {
        public string Title { get; set; }
        public string[] Location { get; set; }
        public string Date { get; set; }
        public List<EventFight> Fights { get; set; }
    }
}