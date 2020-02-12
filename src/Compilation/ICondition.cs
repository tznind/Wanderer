namespace Wanderer.Compilation
{
    public interface ICondition
    {
        string CsharpCode { get; set; }

        bool IsMet(object o);
    }

    public interface ICondition<T> : ICondition
    {
        
        bool IsMet(T o);
    }
}