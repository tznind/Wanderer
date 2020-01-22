using System;
using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public class Rusty : Adjective
    {
        public Rusty(IHasStats owner) : base(owner)
        {
        }

        public override StatsCollection GetFinalStats(IActor forActor)
        {
            var clone = Owner.BaseStats.Clone();
            clone.SetAll(v=> (Math.Abs(v)/2));

            return base.GetFinalStats(forActor).Clone().Subtract(clone);
        }


        public override IEnumerable<string> GetDescription()
        {
            yield return "Makes worse";
            yield return "Reduces value";
        }
    }
}