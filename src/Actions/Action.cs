using System;
using System.Linq;
using Newtonsoft.Json;
using Wanderer.Actors;
using Wanderer.Behaviours;

namespace Wanderer.Actions
{
    public abstract class Action : HasStats,IAction
    {
        [JsonIgnore]
        public abstract char HotKey {get;}
        
        public IHasStats Owner { get; set; }

        /// <summary>
        /// Initializes action with a default <see cref="HasStats.Name"/> based on the class name
        /// </summary>
        protected Action(IHasStats owner)
        {
            Name = GetType().Name.Replace("Action", "");
            Owner = owner;
        }

        /// <summary>
        /// Prompts for all choices and then pushes onto <paramref name="stack"/>
        /// a suitable <see cref="Frame"/> (or not if there are no valid options picked / option
        /// picking is cancelled.
        /// 
        /// <para>Actual resolution of the action should be reserved for the <see cref="Pop"/> method</para>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="actor"></param>
        public abstract void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor);


        /// <summary>
        /// Override to your action once it is confirmed
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        public virtual void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {

        }

        public abstract bool HasTargets(IActor performer);

        public virtual IAction Clone()
        {
            //TODO preserve Owner
            return (IAction) Activator.CreateInstance(GetType(),true);
        }

        public virtual ActionDescription ToActionDescription()
        {
            return new ActionDescription(){HotKey = HotKey, Name = Name};
        }

        public bool AreIdentical(IAction other)
        {
            if (other == null)
                return false;

            return this.Name == other.Name;
        }

        public override string ToString()
        {
            return $"{Name} [{Owner}]";
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
    }
}