using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Compilation;

namespace Wanderer.Factories.Blueprints
{
    /// <summary>
    /// Blueprint that describes how to build one or more <see cref="ICondition"/>
    /// </summary>
    public class ConditionBlueprint
    {
        /// <summary>
        /// Lua code that returns true or false
        /// </summary>
        public string Lua { get; set; }

        /// <summary>
        /// Pass a Guid or Name of something they might have, if they have it then the condition is met
        /// </summary>
        public string Has { get; set; }

        /// <summary>
        /// Pass a Guid or Name, condition is true if the object is it.  This does not include things they have e.g. items, adjectives etc (see <see cref="Has"/> for that)
        /// </summary>
        public string Is {get;set;}

        /// <summary>
        /// Pass a Guid or Name of something.  As long as they don't have it this condition is true
        /// </summary>
        public string HasNot {get;set;}

        /// <summary>
        /// Pass a Guid or Name, condition is true as long as the object is NOT it.  This does not include things they have e.g. items, adjectives etc  (see <see cref="Has"/> for that)
        /// </summary>
        public string IsNot {get;set;}
        
        /// <summary>
        /// Arithmetic expression for a required stat they must have e.g. "Fight > 50"
        /// </summary>
        public string Stat {get;set;}

        /// <summary>
        /// Arithmetic expression for a required custom variable e.g. "MyCounter > 50" (See <see cref="IHasStats.V"/>)
        /// </summary>
        public string Variable {get;set;}

        /// <summary>
        /// Apply the check (Has, Is, Stat etc) to the given object (default is Aggressor - the acting thing).  Options include Room (where room the event is taking place), Recipient (who you are talking to) etc
        /// </summary>
        public SystemArgsTarget Check { get; set; }
        
        /// <summary>
        /// Creates one <see cref="ICondition"/> for each configured blueprint option e.g. <see cref="Lua"/> creates a <see cref="ConditionCode"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICondition> Create()
        {
            if(!string.IsNullOrWhiteSpace(Lua))
                yield return new ConditionCode(Lua);

            if (!string.IsNullOrWhiteSpace(Has))
                yield return new HasCondition(Has,Check);

            if (!string.IsNullOrWhiteSpace(HasNot))
                yield return new HasCondition(HasNot,Check) { InvertLogic = true};

            if (!string.IsNullOrWhiteSpace(Is))
                yield return new HasCondition(Is,Check){UseIs = true};

            if (!string.IsNullOrWhiteSpace(IsNot))
                yield return new HasCondition(IsNot,Check){InvertLogic = true, UseIs = true};

            if(!string.IsNullOrWhiteSpace(Stat))
                yield return new StatCondition(Stat,Check);
            
            if(!string.IsNullOrWhiteSpace(Variable))
                yield return new VariableCondition(Variable,Check);
            
                
        }
    }
}