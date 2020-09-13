using System;
using System.Collections.Generic;

namespace aspnetcore_htmlParser.Models {
    public class Stat {
        public int TotalInfected { get; set; }
        public int TotalRecovered { get; set; }
        public int TotalDeath { get; set; }
        public double MortalityRate { get; set; }
        public double DeathPerMillion { get; set; }
    }
}