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

        /// <summary>
        /// Calculates how susceptible something is based on the current
        /// resistances (<see cref="Immune"/> etc).
        /// </summary>
        /// <param name="s"></param>
        /// <returns>0 for immunity, 1 for normal. 0.5 for resist and 2 for vulnerable</returns>
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

        /// <summary>
        /// Calculates how susceptible something is based on the current
        /// resistances (<see cref="Immune"/> etc).  This overload lets you
        /// omit item adjectives
        /// </summary>
        /// <param name="a"></param>
        /// <param name="includeItems">True to include items you are carrying when
        /// considering if you meet the immunities</param>
        /// <returns>0 for immunity, 1 for normal. 0.5 for resist and 2 for vulnerable</returns>
        public double Calculate(IActor a, bool includeItems)
        {
            //if you are immune
            if (Immune.Any(i=>a.Has(i,includeItems)))
                return 0;

            bool resist = Resist.Any(i => a.Has(i, includeItems));
            bool vuln = Vulnerable.Any(i => a.Has(i, includeItems));

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