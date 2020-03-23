using System.Collections.Generic;
using Wanderer.Actions;
using Wanderer.Compilation;

namespace Wanderer.Factories.Blueprints
{
    public class ActionBlueprint : HasStatsBlueprint
    {
        public char HotKey {get;set;}

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
        /// What happens when the action is performed.
        /// </summary>
        public List<IEffect> Effect { get; set; } = new List<IEffect>();
    }
}