using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer.Actions
{
    /// <inheritdoc/>
    public abstract class Action : IAction
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public double Attitude { get; set; }

        /// <summary>
        /// Initializes action with a default <see cref="Name"/> based on the class name
        /// </summary>
        protected Action()
        {
            Name = GetType().Name.Replace("Action", "");
        }

        /// <summary>
        /// Resets action state to <see cref="CancellationStatus.NotCancelled"/> and
        /// pushes onto <paramref name="stack"/>.  Overrides should prompt for any
        /// additional setup for maybe executing the command (i.e. in <see cref="Pop"/>)
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="actor"></param>
        public virtual void Push(IUserinterface ui, ActionStack stack,IActor actor)
        {
            stack.Push(new Frame(actor,this));
        }


        /// <summary>
        /// Override to your action once it is confirmed
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        public abstract void Pop(IUserinterface ui, ActionStack stack, Frame frame);

        /// <summary>
        /// Returns <see cref="Name"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}