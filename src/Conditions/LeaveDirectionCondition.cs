using System.Linq;
using StarshipWanderer.Actions;

namespace StarshipWanderer.Conditions
{
    public class LeaveDirectionCondition : ICondition<Leave>
    {
        public Direction[] ConditionalDirections { get; set; }

        public LeaveDirectionCondition(params Direction[] conditionalDirections)
        {
            ConditionalDirections = conditionalDirections;
        }

        public bool IsMet(Leave forTarget)
        {
            return ConditionalDirections.Contains(forTarget.Direction);
        }
    }
}