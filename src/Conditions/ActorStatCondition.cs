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
            
            //todo: setting this to final stats causes stack overflow (cannot
            //check if you meet the stats requirements because that involves
            //totaling up your stats!
            return base.IsMet(forTarget.BaseStats);
        }

        public string? SerializeAsConstructorCall()
        {
            return $"ActorStat({ToCheck},{Comparison},{Value})";
        }

        public override string ToString()
        {
            return $"{ToCheck} {Comparison} {Value}";
        }
    }
}