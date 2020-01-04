using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives.ActorOnly;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public class Attractive : Adjective
    {
        public Attractive(IHasStats actor) : base(actor)
        {
            BaseStats[Stat.Coerce] = 15;
        }

        public override bool IsActive()
        {
            //Being attractive makes you better at coercion as long as you do not have an active injury
            return !Owner.Adjectives.Any(a => a is Injured i && i.IsActive());
        }
    }
}