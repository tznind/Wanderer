using System.Collections.Generic;
using System.Linq;
using Wanderer.Behaviours;
using Wanderer.Systems;

namespace Wanderer.Factories.Blueprints
{
    /// <summary>
    /// Describes how to build a <see cref="BehaviourEventHandler"/> for a given event on a <see cref="Behaviour"/> e.g. <see cref="IBehaviour.OnEnter"/>
    /// </summary>
    public class BehaviourEventHandlerBlueprint
    {
        /// <summary>
        /// Pre conditions to check when the event occurs before launching off the <see cref="Effect"/>.  If multiple conditions are specified then all must be met
        /// </summary>
        public List<ConditionBlueprint> Condition {get;set;}

        /// <summary>
        /// Things that should happen when the event handler occurs and the <see cref="Condition"/> are met e.g. cause damage to someone, launch some dialogue etc
        /// </summary>
        public List<EffectBlueprint> Effect {get;set;}

        /// <summary>
        /// Builds the blueprint into a runtime instance
        /// </summary>
        /// <returns></returns>
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