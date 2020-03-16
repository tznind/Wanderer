using Wanderer.Actors;
using Wanderer.Behaviours;

namespace Wanderer.Actions
{
    public interface IAction : IHasStats,IAreIdentical<IAction>
    {
        /// <summary>
        /// The person or object granting the action
        /// </summary>
        IHasStats Owner { get; set; }

        /// <summary>
        /// The hotkey for the action, should be a letter in the Name
        /// </summary>
        char HotKey {get;}

        /// <summary>
        /// When implemented results in pushing the current command onto the <paramref name="stack"/>
        /// Can result in no change to <paramref name="stack"/> if the action is cancelled e.g. as
        /// a result of asking the user a question through <paramref name="ui"/>.
        /// </summary>
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
        /// Creates a new copy of this action
        /// </summary>
        /// <returns></returns>
        IAction Clone();

        /// <summary>
        /// Returns the shared user understandable common description of all instances
        /// of this action.  E.g. each FightAction is separate but they share the same
        /// description.
        /// </summary>
        /// <returns></returns>
        ActionDescription ToActionDescription();
    }
}