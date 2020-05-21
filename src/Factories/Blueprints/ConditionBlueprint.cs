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
        /// Pass a Guid or Name of something.  As long as they don't have it this condition is true
        /// </summary>
        public string HasNot {get;set;}

        /// <summary>
        /// Arithmetic expression for a required stat they must have e.g. "Fight > 50"
        /// </summary>
        public string Stat {get;set;}
        
        /// <summary>
        /// Creates one <see cref="ICondition"/> for each configured blueprint option e.g. <see cref="Lua"/> creates a <see cref="ConditionCode"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICondition> Create()
        {
            if(!string.IsNullOrWhiteSpace(Lua))
                yield return new ConditionCode(Lua);

            if (!string.IsNullOrWhiteSpace(Has))
                yield return new HasCondition(Has);

            if (!string.IsNullOrWhiteSpace(HasNot))
                yield return new HasCondition(HasNot)
                {
                    InvertLogic = true
                };

            if(!string.IsNullOrWhiteSpace(Stat))
                yield return new StatCondition(Stat);
        }
    }
}