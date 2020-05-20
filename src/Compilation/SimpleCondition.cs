using System;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public abstract class SimpleCondition<T> : ICondition<T>
    {
        public virtual bool IsMet(IWorld world, T o)
        {
            return IsMetImpl(world,GetObjectToTest(o));
        }

        protected abstract  bool IsMetImpl(IWorld world, IHasStats hasStats);

        protected virtual IHasStats GetObjectToTest(T o)
        {
            if(o == null)
                throw new ArgumentNullException(nameof(o));

            if (o is IHasStats i)
                return i;

            if (o is SystemArgs s)
                return (s.AggressorIfAny ?? s.Recipient);

            throw new NotSupportedException($"Unknown T type {typeof(T)}");
        }
    }
}