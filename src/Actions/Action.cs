using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer.Actions
{
    public abstract class Action : IAction
    {
        public IWorld World { get; set; }
        public IActor PerformedBy { get; set; }
        public string Name { get; set; }

        public CancellationStatus Cancelled { get; set; }  = CancellationStatus.NotCancelled;

        protected Action(IWorld world, IActor performedBy)
        {
            World = world;
            PerformedBy = performedBy;
            Name = GetType().Name.Replace("Action", "");
        }

        /// <summary>
        /// Override to setup your action to run (push it onto the <paramref name="stack"/>)
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        public abstract void Push(IUserinterface ui,ActionStack stack);

        
        /// <summary>
        /// Override to your action once it is confirmed
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        public abstract void Pop(IUserinterface ui,ActionStack stack);
    }
}