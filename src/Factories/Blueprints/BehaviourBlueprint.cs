using Wanderer.Actions;
using Wanderer.Behaviours;

namespace Wanderer.Factories.Blueprints
{
    /// <summary>
    /// Describes how  to build a <see cref="Behaviour"/>
    /// </summary>
    public class BehaviourBlueprint: HasStatsBlueprint
    {
        /// <summary>
        /// Event handler to trigger on the the behaviour whenever an action is put on the stack
        /// </summary>
        public BehaviourEventHandlerBlueprint OnPush {get;set;}
        
        /// <summary>
        /// Event handler to trigger on the the behaviour whenever an action begins successful resolution (the action stack was pushed and nobody removed it from the stack before resolution)
        /// </summary>
        public BehaviourEventHandlerBlueprint OnPop {get;set;}

        /// <summary>
        /// Event handler to call after the player has taken an action and all other world actors have responded and the round is ending
        /// </summary>
        public BehaviourEventHandlerBlueprint OnRoundEnding {get;set;}

        /// <summary>
        /// Event handler to call when any actor performs the <see cref="LeaveAction"/> and successfully enters a new room.  Note that this gets called for every person in the worlds population that moves not just the behaviour's owner
        /// </summary>
        public BehaviourEventHandlerBlueprint OnEnter { get; set; }
    }
}