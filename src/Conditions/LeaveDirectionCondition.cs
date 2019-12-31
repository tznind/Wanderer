using System.Linq;
using StarshipWanderer.Actions;

namespace StarshipWanderer.Conditions
{
    public class LeaveDirectionCondition : ICondition<Frame>
    {
        public Direction[] ConditionalDirections { get; set; }

        public LeaveDirectionCondition(params Direction[] conditionalDirections)
        {
            ConditionalDirections = conditionalDirections;
        }

        public bool IsMet(Frame forTarget)
        {
            return forTarget is LeaveFrame f && ConditionalDirections.Contains(f.Direction);
        }
    }
}