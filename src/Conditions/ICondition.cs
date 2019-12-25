namespace StarshipWanderer.Conditions
{
    public interface ICondition<in T>
    {
        bool IsMet(T forTarget);
    }
 
}