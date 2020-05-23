using System;
using Wanderer.Factories.Blueprints;
using Wanderer.Rooms;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    /// <summary>
    /// Condition which checks for whether something has a give other thing (by Name, Identifier etc).  E.g. a Gun might Has Rusty
    /// </summary>
    public class HasCondition : Condition
    {
        /// <summary>
        /// Name or Identifier of something which you want to test for the presence of e.g. for a Room a condition could check for 'Dark'
        /// </summary>
        public string Requirement { get; }

        /// <summary>
        /// Set to true to require the absence of the <see cref="Requirement"/> for the condition to be met.  Default is false.
        /// </summary>
        public bool InvertLogic {get;set;}
        
        /// <summary>
        /// True to use <see cref="IHasStats.Is(string)"/> instead of <see cref="IHasStats.Has(string)"/>.  Default is false
        /// </summary>
        public bool UseIs { get; internal set; } 

        public HasCondition(string required,SystemArgsTarget check) :base(check)
        {
            Requirement = required;
        }

        /// <inheritdoc />
        public override bool IsMet(IWorld world, SystemArgs o)
        {
            var toCheck = o.GetTarget(Check);

            return (UseIs ? toCheck.Is(Requirement) :toCheck.Has(Requirement)) == !InvertLogic;
        }

        public override string ToString()
        {
            return Check + " " + Requirement;
        }
    }
}