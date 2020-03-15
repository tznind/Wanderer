using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;

namespace Wanderer.Systems
{
    public class Resistances
    {
        public List<string> Vulnerable {get; set; } = new List<string>();

        public List<string> Resist {get; set; } = new List<string>();
        
        public List<string> Immune {get; set; } = new List<string>();

        public double Calculate(IHasStats s)
        {
            //if you are immune
            if (Immune.Any(s.Has))
                return 0;

            bool resist = Resist.Any(s.Has);
            bool vuln = Vulnerable.Any(s.Has);

            if (resist && vuln)
                return 1;
            if (resist)
                return 0.5;
            if (vuln)
                return 2;
            
            //neither resistant nor vulnerable
            return 1;
        }
    }
}