using System;
using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives.RoomOnly
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

            return base.GetFinalStats(forActor).Subtract(clone);
        }


        public override IEnumerable<string> GetDescription()
        {
            yield return "Makes worse";
            yield return "Reduces value";
        }
    }

    public class Dark : Adjective
    {
        public Dark(IPlace owner) : base(owner)
        {
            BaseStats[Stat.Fight] = -10;
        }

        public override StatsCollection GetFinalStats(IActor forActor)
        {
            //return 0 if actor has a light
            if(forActor.Has<Light>(true))
                return new StatsCollection();

            return base.GetFinalStats(forActor);
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Reduces Fight";
        }
    }
}
