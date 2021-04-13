using System.Collections.Generic;

namespace FightmetricRdsCaScraper
{
    public class FighterFight
    {
        public string WL { get; set; }
        public string Weight { get; set; }
        public bool Belt { get; set; }
        public Dictionary<string, string> Fighters { get; set; }
        public string KD { get; set; }
        public string STR { get; set; }
        public string TD { get; set; }
        public string SUB { get; set; }
        public string EventName { get; set; }
        public string EventHref { get; set; }
        public string Method { get; set; }
        public string Round { get; set; }
        public string Time { get; set; }
        public string StatsHref { get; set; }
    }
}