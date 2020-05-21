using Wanderer.Behaviours;

namespace Wanderer.Factories.Blueprints
{
    public class BehaviourBlueprint: HasStatsBlueprint
    {
        public BehaviourEventHandlerBlueprint OnPush {get;set;}
        public BehaviourEventHandlerBlueprint OnPop {get;set;}
        public BehaviourEventHandlerBlueprint OnRoundEnding {get;set;}
        public BehaviourEventHandlerBlueprint OnEnter { get; set; }
    }
}