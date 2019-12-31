using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer.Actions
{
    public interface IAction
    {
        /// <summary>
        /// The human readable name of the action which can be undertaken
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// When implemented results in pushing the current command onto the <paramref name="stack"/>
        /// Can result in no change to <paramref name="stack"/> if the action is cancelled e.g. as
        /// a result of asking the user a question through <paramref name="ui"/>.
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="performer"></param>
        void Push(IUserinterface ui, ActionStack stack,IActor performer);

        /// <summary>
        /// When implemented executes the action after it has been confirmed by the full
        /// <paramref name="stack"/> execution (e.g. nobody cancelled you etc).
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        void Pop(IUserinterface ui,ActionStack stack,Frame frame);
    }
}