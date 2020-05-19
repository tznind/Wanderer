using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Compilation;

namespace Wanderer.Factories.Blueprints
{
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
        
        public IEnumerable<ICondition<T>> Create<T>()
        {
            if(!string.IsNullOrWhiteSpace(Lua))
                yield return new ConditionCode<T>(Lua);

            if (!string.IsNullOrWhiteSpace(Has))
                yield return new HasCondition<T>(Has);

            //TODO: build other conditions here
        }
    }
}