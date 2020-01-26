using StarshipWanderer.Systems;

namespace StarshipWanderer.Conditions
{
    public interface ICondition<in T> : IConditionBase
    {
        bool IsMet(T forTarget);
    }
 
}