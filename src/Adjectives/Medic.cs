using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public class Medic : Adjective
    {
        public Medic(IHasStats owner):base(owner)
        {
            BaseActions.Add(new HealAction());
        }
        
        public override IEnumerable<string> GetDescription()
        {
            yield return "Allows healing injuries";
        }
    }
}