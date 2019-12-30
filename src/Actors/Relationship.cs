using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    class Relationship
    {
        public int Attitude { get; set; }

        public Relationship(IActor observer, IActor observed)
        {
            var observerStats = observer.GetFinalStats();
            var observedStats = observed.GetFinalStats();

        }
    }
}
