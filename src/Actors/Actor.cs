using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarshipWanderer.Actions;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    public class Actor : IActor
    {
        public string Name { get; set; }
        
        public HashSet<IAction> BaseActions { get; set; } = new HashSet<IAction>();

        public HashSet<IAdjective> Adjectives { get; set; } = new HashSet<IAdjective>();

        /// <summary>
        /// Actors stats before applying any modifiers
        /// </summary>
        public StatsCollection BaseStats { get; set; } = new StatsCollection();

        public HashSet<IBehaviour> BaseBehaviours { get; set; } = new HashSet<IBehaviour>();

        public Actor(string name)
        {
            Name = name;
        }

        public void AddBehaviour(IBehaviour b)
        {
            BaseBehaviours.Add(b);
        }

        public IEnumerable<IAction> GetFinalActions(IWorld world,IPlace actorsPlace)
        {
            yield return new Leave(world, this, actorsPlace);
            yield return new FightAction(world,this);

            foreach (var a in BaseActions.Union(Adjectives.SelectMany(a=>a.Actions)).Distinct()) 
                yield return a;
        }

        public virtual bool Decide<T>(IUserinterface ui, string title, string body,out T chosen, T[] options, int attitude)
        {
            chosen = default(T);
            return false;
        }

        public virtual void Move(IWorld world, IPlace currentLocation, IPlace newLocation)
        {
            currentLocation.Occupants.Remove(this);
            newLocation.Occupants.Add(this);
        }

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
