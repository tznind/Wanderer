using System;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class HasCondition<T> : ICondition<T>
    {
        public string Requirement { get; }

        public HasCondition(string required)
        {
            Requirement = required;
        }

        public bool IsMet(IWorld world, T o)
        {
            if(o == null)
                throw new ArgumentNullException(nameof(o));

            if (o is IHasStats i)
                return i.Has(Requirement);

            if (o is SystemArgs s)
                return (s.AggressorIfAny ?? s.Recipient).Has(Requirement);

            throw new NotSupportedException($"Unknown T type {typeof(T)}");
        }
    }
}