using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Compilation;

namespace Wanderer.Actions
{
    /// <summary>
    /// An activity that an <see cref="IActor"/> can perform.  May or may not take a full round to complete.  Action execution involves picking targets and then pushing the resulting selection as a <see cref="Frame"/> onto an <see cref="ActionStack"/> where it may be resolved later or cancelled.  Both player and NPC alike pick <see cref="IAction"/>.
    /// </summary>
    public interface IAction : IHasStats
    {
        /// <summary>
        /// The person or object granting the action
        /// </summary>
        IHasStats Owner { get; set; }

        /// <summary>
        /// The hotkey for the action, should be a letter in the Name
        /// </summary>
        char HotKey {get; set;}

        /// <summary>
        /// Effects that happen when the action is performed.  These only fire if the action successfully resolves (i.e. is not cancelled).  Effects are applied before any normal parts of the action are resolved (e.g. exchanging damage during a <see cref="FightAction"/>)
        /// </summary>
        List<IEffect> Effect {get;set;}

        /// <summary>
        /// What can be targeted by the action.  If an action has no targets then
        /// it is assumed to always be available
        /// </summary>
        List<IActionTarget> Targets { get; set; }
        
        /// <summary>
        /// If there are <see cref="Targets"/> configured then this
        /// is the message to show when prompting to pick them
        /// </summary>
        string TargetPrompt { get; set; }

        /// <summary>
        /// When implemented results in pushing the current command onto the <paramref name="stack"/>
        /// Can result in no change to <paramref name="stack"/> if the action is cancelled e.g. as
        /// a result of asking the user a question through <paramref name="ui"/>.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="performer"></param>
        void Push(IWorld world,IUserinterface ui, ActionStack stack,IActor performer);

        /// <summary>
        /// When implemented executes the action after it has been confirmed by the full
        /// <paramref name="stack"/> execution (e.g. nobody cancelled you etc).
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame);

        /// <summary>
        /// Returns true if there are legal targets for the ability.
        /// </summary>
        /// <param name="performer"></param>
        /// <returns></returns>
        bool HasTargets(IActor performer);
        /// <summary>
        /// Returns true if there are legal targets for the ability.
        /// </summary>
        /// <param name="performer"></param>
        /// <returns></returns>
        IEnumerable<IHasStats> GetTargets(IActor performer);
        
        /// <summary>
        /// Returns the shared user understandable common description of all instances
        /// of this action.  E.g. each FightAction is separate but they share the same
        /// description.
        /// </summary>
        /// <returns></returns>
        ActionDescription ToActionDescription();
    }
}