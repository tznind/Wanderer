using System.Collections.Generic;
using Wanderer.Compilation;

namespace Wanderer.Factories.Blueprints
{
    public class ConditionBlueprint
    {
        public string Lua { get; set; }

        public IEnumerable<ICondition<T>> Create<T>()
        {
            if(!string.IsNullOrWhiteSpace(Lua))
                yield return new ConditionCode<T>(Lua);

            //TODO: build other conditions here
        }
    }
}