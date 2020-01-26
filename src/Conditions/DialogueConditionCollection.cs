using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Conditions
{
    public class DialogueConditionCollection : List<ICondition<SystemArgs>>
    {
        public DialogueConditionCollection()
        {
            
        }
        public DialogueConditionCollection(params ICondition<SystemArgs>[] conditions)
        {
            if(conditions.Any())
                AddRange(conditions);
        }
    }
}