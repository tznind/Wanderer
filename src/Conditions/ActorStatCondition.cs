using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Conditions
{
    /// <summary>
    /// Condition which checks the stats of an <see cref="IActor"/>
    /// </summary>
    public class ActorStatCondition : StatCondition, ICondition<IActor>
    {
        public ActorStatCondition(Stat toCheck,Comparison comparison, double value):base(toCheck,comparison,value)
        {
        }

        public bool IsMet(IActor forTarget)
        {
            return base.IsMet(forTarget.GetFinalStats());
        }
    }
}