namespace StarshipWanderer.Conditions
{
    /// <summary>
    /// Condition that is never met
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NeverCondition<T> : ICondition<T>
    {
        public bool IsMet(T forTarget)
        {
            return false;
        }

        public string? SerializeAsConstructorCall()
        {
            return "Never()";
        }
    }
}