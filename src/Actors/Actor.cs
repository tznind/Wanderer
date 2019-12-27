using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    public class Actor : IActor
    {
        public string Name { get; set; }
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
