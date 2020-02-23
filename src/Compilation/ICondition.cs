namespace Wanderer.Compilation
{
    public interface ICondition
    {
        bool IsMet(IWorld world, object o);
    }

    public interface ICondition<T> : ICondition
    {
        
        bool IsMet(IWorld world,T o);
    }
}