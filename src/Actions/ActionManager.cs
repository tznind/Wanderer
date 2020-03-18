using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;

namespace Wanderer.Actions
{
    /// <summary>
    /// Determines which actions a player is capable of and what
    /// valid targets can be picked for them
    /// </summary>
    public class ActionManager
    {
        public IEnumerable<ActionDescription> GetTypes(IActor aggressor,bool mustHaveTargets)
        {
            return aggressor.GetFinalActions(aggressor)
                .Where(a=>!mustHaveTargets || a.HasTargets(aggressor))
                .Select(a=>a.ToActionDescription())
                .Distinct().ToList();
        }
        public List<IAction> GetInstances(IActor aggressor,ActionDescription type, bool mustHaveTargets)
        {
            if(type == null)
                return new List<IAction>();

            return aggressor.GetFinalActions(aggressor)
                .Where(a=>!mustHaveTargets || a.HasTargets(aggressor))
                .Where(type.Matches).ToList();
        }
    }
}