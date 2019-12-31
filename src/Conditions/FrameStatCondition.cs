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
        
        public FrameStatCondition(Stat toCheck,Comparison comparison, int value):base(toCheck,comparison,value)
        {
        }

        public bool IsMet(Frame forTarget)
        {
            return base.IsMet(forTarget.PerformedBy.GetFinalStats());
        }
    }
}