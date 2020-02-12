using System;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Behaviours;

namespace Wanderer.Actions
{
    /// <inheritdoc/>
    public abstract class Action : IAction
    {
        /// <inheritdoc/>
        public string Name { get; set; }

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
        public abstract void Push(IUserinterface ui, ActionStack stack, IActor actor);


        /// <summary>
        /// Override to your action once it is confirmed
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        public abstract void Pop(IUserinterface ui, ActionStack stack, Frame frame);

        public abstract bool HasTargets(IActor performer);

        public bool AreIdentical(IAction other)
        {
            if (other == null)
                return false;

            return this.Name == other.Name;
        }

        /// <summary>
        /// Returns <see cref="Name"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        
        protected void ShowNarrative(IUserinterface ui,IActor actor,string title, string fluff, string technical,Guid round)
        {
            var narrative = new Narrative(actor, title, fluff, technical,round);
            narrative.Show(ui);
        }

        /// <summary>
        /// Returns all values of the Enum T except for "None"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T[] GetValues<T>() where T:Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .Where(t => !string.Equals(t.ToString(), "None", StringComparison.CurrentCultureIgnoreCase)).ToArray();
        }


        protected bool Equals(Action other)
        {
            return other.GetType() == GetType();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Action) obj);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}