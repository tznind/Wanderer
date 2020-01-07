using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public class Coerced : Adjective
    {
        public Coerced(IActor owner) : base(owner)
        {
            //don't show this Adjective in it's name
            IsPrefix = false;
            BaseStats[Stat.Initiative] = 10000;
            BaseBehaviours.Add(new ExpiryBehaviour(this, 1));
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Forced to perform a specific action";
        }
    }
}