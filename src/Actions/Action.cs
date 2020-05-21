using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;
using Wanderer.Rooms;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    /// <summary>
    /// An action that an <see cref="IActor"/> can perform 
    /// </summary>
    public class Action : HasStats,IAction
    {
        /// <summary>
        /// Single key that should allow the player to activate this action (assuming support in a suitable UI)
        /// </summary>
        public char HotKey {get; set;}
        
        /// <summary>
        /// Where the action comes from, could be an <see cref="IActor"/> but could equally be owned by a <see cref="IRoom"/> (e.g. 'push the red button')
        /// </summary>
        public IHasStats Owner { get; set; }


        /// <summary>
        /// How kind is the action? before picking any targets
        /// </summary>
        public double Attitude {get;set;}

        /// <summary>
        /// What can be targetted by the action
        /// </summary>
        public List<IActionTarget>  Targets {get;set;} = new List<IActionTarget>();

        /// <inheritdoc />
        public string TargetPrompt { get; set; }


        /// <inheritdoc />
        public List<IEffect> Effect {get;set;} = new List<IEffect>();
        
        /// <summary>
        /// Initializes action with a default <see cref="HasStats.Name"/> based on the class name
        /// </summary>
        public Action(IHasStats owner)
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
        public virtual void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            var targets = GetTargets(actor).ToArray();
            IHasStats target = Owner;

            if(targets.Length >= 1)
                if(!actor.Decide(ui,Name,TargetPrompt,out target, targets,Attitude))
                    return;
            
            stack.Push(new Frame(actor,this,GetAttitude(actor,target) ?? Attitude){TargetIfAny = target});
        }

        /// <summary>
        /// Return the attitude given the picked <paramref name="target"/> (if null is returned then fallback of <see cref="Attitude"/> is used)
        /// </summary>
        /// <param name="performer"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected virtual double? GetAttitude(IActor performer, IHasStats target)
        {
            return null;
        }


        /// <summary>
        /// Override to your action once it is confirmed
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        public virtual void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            foreach (var e in Effect) 
                e.Apply(new ActionFrameSystemArgs(this,world, ui, stack, frame));

            PopImpl(world, ui, stack, frame);
        }

        /// <summary>
        /// Override to unpack your pushed <paramref name="frame"/> and make changes to the world to reflect what your action should have done.  This method is called after invoking all <see cref="Effect"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        protected virtual void PopImpl(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            
        }

        /// <inheritdoc />
        public virtual bool HasTargets(IActor performer)
        {
            if (!Targets.Any())
                return true;

            return GetTargets(performer).Any();
        }

        /// <inheritdoc />
        public virtual IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            if(Targets.Any())
            {
                var args = new SystemArgs(performer.CurrentLocation.World,null,0,performer,Owner,Guid.Empty);
                return Targets.SelectMany(t=>t.Get(args)).Distinct();
            }

            return new IHasStats[0];
        }

        /// <summary>
        /// Creates a new copy, if your action does not have a default constructor then you will have to override this method
        /// </summary>
        /// <returns></returns>
        public virtual IAction Clone()
        {
            var a = (IAction) Activator.CreateInstance(GetType(), true);
            a.Owner = Owner;
            return a;
        }


        /// <inheritdoc />
        public virtual ActionDescription ToActionDescription()
        {
            return new ActionDescription(){HotKey = HotKey, Name = Name};
        }

        /// <summary>
        /// True if the user would consider this and the <paramref name="other"/> the same (even if they have different <see cref="Owner"/>)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool AreIdentical(IAction other)
        {
            if (other == null)
                return false;

            return this.Name == other.Name;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name} [{Owner}]";
        }

        
        /// <summary>
        /// Creates a new <see cref="Narrative"/> (non dialogue story piece) and runs it in the <paramref name="ui"/>
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="actor"></param>
        /// <param name="title"></param>
        /// <param name="fluff">How to describe what is happening as a story description e.g. "The liquid tastes sweet and makes you feel strong"</param>
        /// <param name="technical">How to describe what is happening in logs e.g. "+10 Strength"</param>
        /// <param name="round"></param>
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