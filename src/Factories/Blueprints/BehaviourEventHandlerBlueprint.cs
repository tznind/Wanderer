using System.Collections.Generic;
using System.Linq;
using Wanderer.Behaviours;
using Wanderer.Systems;

namespace Wanderer.Factories.Blueprints
{
    public class BehaviourEventHandlerBlueprint
    {
        public List<ConditionBlueprint> Condition {get;set;}

        public List<EffectBlueprint> Effect {get;set;}

        public BehaviourEventHandler Create()
        {
            var handler = new BehaviourEventHandler();
            
            if(Condition != null)
                handler.Condition.AddRange(Condition.SelectMany(c=>c.Create()));
            if(Effect != null)
                handler.Effect.AddRange(Effect.SelectMany(e=>e.Create()));

            return handler;
        }
    }
}