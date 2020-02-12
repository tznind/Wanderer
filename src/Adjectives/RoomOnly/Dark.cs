using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Places;
using Wanderer.Stats;

namespace Wanderer.Adjectives.RoomOnly
{
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
