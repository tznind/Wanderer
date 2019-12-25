using StarshipWanderer.Actions;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Conditions
{
    /// <summary>
    /// Condition which checks the stats of an <see cref="IAction.PerformedBy"/>
    /// </summary>
    public class ActionStatCondition : StatCondition, ICondition<IAction> 
    {
        
        public ActionStatCondition(Stat toCheck,Comparison comparison, int value):base(toCheck,comparison,value)
        {
        }

        public bool IsMet(IAction forTarget)
        {
            return base.IsMet(forTarget.PerformedBy.GetFinalStats());
        }
    }
}