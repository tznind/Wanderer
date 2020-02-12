using System.Collections.Generic;
using System.Linq;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Stats;

namespace Wanderer.Adjectives
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