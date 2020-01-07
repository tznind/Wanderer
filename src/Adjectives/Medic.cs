using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public class Medic : Adjective
    {
        public Medic(IHasStats owner):base(owner)
        {
        }

        public override IEnumerable<IAction> GetFinalActions(IActor forActor)
        {
            if (forActor.GetFinalStats()[Stat.Savvy] >= 10)
                return base.GetFinalActions(forActor).Union(new[] {new HealAction()});
            
            return base.GetFinalActions(forActor);
        }
    }
}