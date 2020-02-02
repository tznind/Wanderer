using StarshipWanderer.Actions;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Conditions
{
    /// <summary>
    /// Condition which checks the stats of an <see cref="Frame.PerformedBy"/>
    /// </summary>
    public class FrameStatCondition : StatCondition, ICondition<Frame> 
    {
        
        public FrameStatCondition(Stat toCheck,Comparison comparison, double value):base(toCheck,comparison,value)
        {
        }

        public bool IsMet(Frame forTarget)
        {
            //todo: setting this to final stats causes stack overflow (cannot
            //check if you meet the stats requirements because that involves
            //totaling up your stats!
            return base.IsMet(forTarget.PerformedBy.BaseStats);
        }

        public string? SerializeAsConstructorCall()
        {
            return $"FrameStat({ToCheck},{Comparison},{Value})";
        }
    }
}