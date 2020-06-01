using System.Collections.Generic;
using Wanderer.Actions;
using Wanderer.Compilation;

namespace Wanderer.Factories.Blueprints
{
    /// <summary>
    /// Describes an activity a player or npc can perform
    /// </summary>
    public class ActionBlueprint : HasStatsBlueprint
    {
        /// <summary>
        /// The hotkey the user should be able to press (assuming UI support) to activate this action
        /// </summary>
        public char? HotKey {get;set;}

        /// <summary>
        /// Name of the root Action type to base the blueprint on e.g. FightAction
        /// </summary>
        public string Type {get;set;}

        /// <summary>
        /// How kind is the action? before picking any targets
        /// </summary>
        public double Attitude {get;set;}
        
        /// <summary>
        /// What can be targetted by the action
        /// </summary>
        public List<IActionTarget>  Targets {get;set;}

        /// <summary>
        /// If there are <see cref="Targets"/> configured then this
        /// is the message to show when prompting to pick them
        /// </summary>
        public string TargetPrompt { get; set; }

        /// <summary>
        /// Effects that happen when the action is performed.  These only fire if the action successfully resolves (i.e. is not cancelled).  Effects are applied before any normal parts of the action are resolved (e.g. exchanging damage during a <see cref="FightAction"/>)
        /// </summary>
        public List<EffectBlueprint> Effect { get; set; } = new List<EffectBlueprint>();
    }
}