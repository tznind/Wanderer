﻿namespace StarshipWanderer.Conditions
{
    /// <summary>
    /// Condition that is always met
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AlwaysCondition<T> : ICondition<T>
    {
        public bool IsMet(T forTarget)
        {
            return true;
        }

        public string? SerializeAsConstructorCall()
        {
            return $"AlwaysCondition<{typeof(T).Name}>()";
        }
    }
}