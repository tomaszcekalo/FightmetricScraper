using System.Collections.Generic;

namespace FightmetricRdsCaScraper
{
    public class EventFight
    {
        public List<EventFighter> Fighters { get; set; }
        public string Weight { get; set; }
        public bool Belt { get; set; }
        public List<string> KnockDowns { get; set; }
        public List<string> Strikes { get; set; }
        public List<string> Takedowns { get; set; }
        public List<string> SubmissionAttempts { get; set; }
        public List<string> Method { get; set; }
        public string Round { get; set; }
        public string Time { get; set; }
        public string StatsHref { get; set; }
    }
}