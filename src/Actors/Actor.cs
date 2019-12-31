using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using StarshipWanderer.Actions;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    /// <inheritdoc/>
    public abstract class Actor : IActor
    {
        /// <inheritdoc/>
        public IPlace CurrentLocation { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public HashSet<IAction> BaseActions { get; set; } = new HashSet<IAction>();

        /// <inheritdoc/>
        public HashSet<IAdjective> Adjectives { get; set; } = new HashSet<IAdjective>();

        /// <inheritdoc/>
        public StatsCollection BaseStats { get; set; } = new StatsCollection();

        /// <inheritdoc/>
        public HashSet<IBehaviour> BaseBehaviours { get; set; } = new HashSet<IBehaviour>();

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
        }

        /// <summary>
        /// Registers a new way of responding to events
        /// </summary>
        /// <param name="b"></param>
        public virtual void AddBehaviour(IBehaviour b)
        {
            BaseBehaviours.Add(b);
        }

        /// <summary>
        /// Returns all <see cref="IAction"/> which the <see cref="Actor"/> can undertake in it's <see cref="CurrentLocation"/> (this includes 
        /// <see cref="BaseActions"/> but also any location or item specific actions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IAction> GetFinalActions()
        {
            yield return new Leave();
            yield return new FightAction();

            foreach (var a in BaseActions.Union(Adjectives.SelectMany(a=>a.Actions)).Distinct()) 
                yield return a;
        }

        public abstract bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options,
            int attitude);

        public virtual void Move(IPlace newLocation)
        {
            CurrentLocation = newLocation;
        }

        public abstract void Kill(IUserinterface ui);

        public IEnumerable<IBehaviour> GetFinalBehaviours()
        {
            return BaseBehaviours.Union(Adjectives.SelectMany(a=>a.Behaviours)).Distinct();
        }

        public StatsCollection GetFinalStats()
        {
            var clone = BaseStats.Clone();

            foreach (var adjective in Adjectives.Where(a=>a.IsActive())) 
                clone.Add(adjective.Modifiers);

            return clone;
        }

        public override string ToString()
        {
            return Name ?? "Unnamed Actor";
        }
    }
}
