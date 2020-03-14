using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Stats;

namespace Wanderer.Adjectives
{
    public class Attractive : Adjective
    {
        public Attractive(IHasStats actor) : base(actor)
        {
            BaseStats[Stat.Coerce] = 15;
        }

        public override StatsCollection GetFinalStats(IActor forActor)
        {
            
            //Being attractive makes you better at coercion as long as you do not have a head injury
            if(forActor.Has<Injured>(false,i=>i.Region == InjuryRegion.Head))
                return new StatsCollection();

            return base.GetFinalStats(forActor);
        }
    }
}