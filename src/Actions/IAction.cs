using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer.Actions
{
    public interface IAction
    {
        string Name { get; set; }
        IActor PerformedBy { get; set; }

        CancellationStatus Cancelled { get; set; }
        

        /// <summary>
        /// When implemented results in pushing the current command onto the <paramref name="stack"/>
        /// Can result in no change to <paramref name="stack"/> if the action is cancelled e.g. as
        /// a result of asking the user a question through <paramref name="ui"/>.
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        void Push(IUserinterface ui, ActionStack stack);

        /// <summary>
        /// When implemented executes the action after it has been confirmed by the full
        /// <paramref name="stack"/> execution (e.g. nobody cancelled you etc).
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        void Pop(IUserinterface ui,ActionStack stack);
    }
}