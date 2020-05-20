using System;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    /// <summary>
    /// Condition which checks for whether something has a give other thing (by Name, Identifier etc).  E.g. a Gun might Has Rusty
    /// </summary>
    public class HasCondition<T> : ICondition<T>
    {
        
        /// <summary>
        /// Name or Identifier of something which you want to test for the presence of e.g. for a Room a condition could check for 'Dark'
        /// </summary>
        public string Requirement { get; }

        /// <summary>
        /// Set to true to require the absence of the <see cref="Requirement"/> for the condition to be met.  Default is false.
        /// </summary>
        public bool InvertLogic {get;set;}

        public HasCondition(string required)
        {
            Requirement = required;
        }

        public bool IsMet(IWorld world, T o)
        {
            if(o == null)
                throw new ArgumentNullException(nameof(o));

            if (o is IHasStats i)
                return i.Has(Requirement) == !InvertLogic;

            if (o is SystemArgs s)
                return (s.AggressorIfAny ?? s.Recipient).Has(Requirement) == !InvertLogic;

            throw new NotSupportedException($"Unknown T type {typeof(T)}");
        }
    }
}