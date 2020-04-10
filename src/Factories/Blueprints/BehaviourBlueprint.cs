using Wanderer.Behaviours;

namespace Wanderer.Factories.Blueprints
{
    public class BehaviourBlueprint: HasStatsBlueprint
    {
        public BehaviourEventHandler OnPush {get;set;}
        public BehaviourEventHandler OnPop {get;set;}
        public BehaviourEventHandler OnRoundEnding {get;set;}
        public BehaviourEventHandler OnEnter { get; set; }
    }
}