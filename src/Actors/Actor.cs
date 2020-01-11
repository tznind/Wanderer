using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using StarshipWanderer.Actions;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    /// <inheritdoc cref="IActor"/>
    public abstract class Actor : HasStats,IActor
    {
        public bool Dead { get; set; }

        /// <inheritdoc/>
        public IPlace CurrentLocation { get; set; }

        public HashSet<IItem> Items { get; set; } = new HashSet<IItem>();
        public HashSet<IFaction> FactionMembership { get; set; } = new HashSet<IFaction>();

        /// <summary>
        /// Do not use, internal constructor for JSON serialization
        /// </summary>
        [JsonConstructor]
        protected Actor()
        {

        }

        /// <summary>
        /// Creates a new actor with the given <paramref name="name"/> and adds him to the <paramref name="currentLocation"/> (and world population)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="currentLocation"></param>
        public Actor(string name,IPlace currentLocation)
        {
            Name = name;
            CurrentLocation = currentLocation;
            CurrentLocation.World.Population.Add(this);

            //basic actions everyone can do (by default)
            BaseActions.Add(new Leave());
            BaseActions.Add(new FightAction());
            BaseActions.Add(new PickUpAction());
            BaseActions.Add(new DropAction());
        }

        /// <summary>
        /// Returns all <see cref="IAction"/> which the <see cref="Actor"/> can undertake in it's <see cref="CurrentLocation"/> (this includes 
        /// <see cref="HasStats.BaseActions"/> but also any location or item specific actions.
        /// </summary>
        /// <param name="forActor"></param>
        /// <returns></returns>
        public override IEnumerable<IAction> GetFinalActions(IActor forActor)
        {
            return BaseActions
                .Union(Adjectives.SelectMany(a => a.GetFinalActions(forActor)))
                .Union(CurrentLocation.GetFinalActions(forActor))
                .Union(Items.SelectMany(i => i.GetFinalActions(forActor)));
        }

        public abstract bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options,
            int attitude);

        public virtual void Move(IPlace newLocation)
        {
            CurrentLocation = newLocation;
        }

        public abstract void Kill(IUserinterface ui, Guid round);

        /// <inheritdoc/>
        public IActor[] GetCurrentLocationSiblings()
        {
            return CurrentLocation.World.Population.Where(o => o.CurrentLocation == CurrentLocation && o != this).ToArray();
        }

        public bool Has<T>(bool includeItems) where T : IAdjective
        {
            return Adjectives.Any(a => a is T)
                || includeItems && Items.Any(i=> i.Has<T>(this));
        }

        public bool Has<T>(bool includeItems, Func<T, bool> condition) where T : IAdjective
        {
            return Adjectives.Any(a => a is T t  && condition(t))
                || includeItems && Items.Any(i => i.Has<T>(this,condition));
        }

        public override IEnumerable<IBehaviour> GetFinalBehaviours(IActor forActor)
        {
            return BaseBehaviours
                .Union(Adjectives.SelectMany(a=>a.GetFinalBehaviours(forActor)))
                .Union(CurrentLocation.GetFinalBehaviours(forActor))
                .Union(Items.SelectMany(i=>i.GetFinalBehaviours(forActor)));
        }

        public override StatsCollection GetFinalStats(IActor forActor)
        {
            var clone = BaseStats.Clone();

            foreach (var adjective in Adjectives) 
                clone.Add(adjective.GetFinalStats(forActor));

            clone.Add(CurrentLocation.GetFinalStats(forActor));

            foreach (var item in Items) 
                clone.Add(item.GetFinalStats(forActor));

            return clone;
        }
        
        public StatsCollection GetFinalStats()
        {
            return GetFinalStats(this);
        }

        
        public IEnumerable<IAction> GetFinalActions()
        {
            return GetFinalActions(this);
        }

        
        public IEnumerable<IBehaviour> GetFinalBehaviours()
        {
            return GetFinalBehaviours(this);
        }
    }
}
