namespace Wanderer.Compilation
{
    public interface ICondition
    {
        bool IsMet(object o);
    }

    public interface ICondition<T> : ICondition
    {
        
        bool IsMet(T o);
    }
}