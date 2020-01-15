using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public class Medic : Adjective
    {
        public Medic(IHasStats owner):base(owner)
        {
            BaseActions.Add(new HealAction());

            Condition = new ActorStatCondition(Stat.Savvy, Comparison.GreaterThanOrEqual, 10);
        }

        public ActorStatCondition Condition { get; set; }

        public override IActionCollection GetFinalActions(IActor forActor)
        {
            if (Condition.IsMet(forActor))
                return base.GetFinalActions(forActor);
            
            return new ActionCollection();
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Allows healing injuries";
        }
    }
}