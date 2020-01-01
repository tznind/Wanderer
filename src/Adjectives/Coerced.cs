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
            BaseStats[Stat.Initiative] = 10000;
            BaseBehaviours.Add(new ExpiryBehaviour(this, 1));
        }
    }
}