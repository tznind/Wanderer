using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actions.Coercion;
using Wanderer.Actors;
using Wanderer.Items;

namespace Wanderer.Actions
{
    /// <summary>
    /// Determines which actions a player is capable of and what
    /// valid targets can be picked for them
    /// </summary>
    public class ActionManager
    {
        /// <summary>
        /// Returns human readable descriptions for all valid actions the <paramref name="aggressor"/> could undertake
        /// </summary>
        /// <param name="aggressor"></param>
        /// <param name="mustHaveTargets"></param>
        /// <returns></returns>
        public IEnumerable<ActionDescription> GetTypes(IActor aggressor,bool mustHaveTargets)
        {
            return aggressor.GetFinalActions(aggressor)
                .Where(a=>!mustHaveTargets || a.HasTargets(aggressor))
                .Select(a=>a.ToActionDescription())
                .Distinct().ToList();
        }

        /// <summary>
        /// Returns all <see cref="IAction"/> which match the <paramref name="actionDescription"/> which the <paramref name="aggressor"/> can undertake.
        /// </summary>
        /// <param name="aggressor"></param>
        /// <param name="actionDescription"></param>
        /// <param name="mustHaveTargets">True to only return actions where there is a viable target</param>
        /// <returns></returns>
        public List<IAction> GetInstances(IActor aggressor,ActionDescription actionDescription, bool mustHaveTargets)
        {
            if(actionDescription == null)
                return new List<IAction>();

            return aggressor.GetFinalActions(aggressor)
                .Where(a=>!mustHaveTargets || a.HasTargets(aggressor))
                .Where(actionDescription.Matches).ToList();
        }

        /// <summary>
        /// Modifies the <paramref name="chosen"/> action setting it's choice of target to <paramref name="target"/>
        /// </summary>
        /// <param name="chosen"></param>
        /// <param name="target"></param>
        public void PrimeCommandWithTarget(IAction chosen, IHasStats target)
        {
            if(chosen is FightAction f)
                f.PrimeWithTarget = target as IActor;
            if (chosen is PickUpAction p)
                p.PrimeWithTarget = target as IItem;
            if (chosen is CoerceAction c)
                c.PrimeWithTarget = target as IActor;
        }
    }
}