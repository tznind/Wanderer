using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    public class Actor : IActor
    {
        public string Name { get; set; }

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
            return BaseBehaviours;
        }

        public StatsCollection GetFinalStats()
        {
            return BaseStats;
        }

        public override string ToString()
        {
            return Name ?? "Unnamed Actor";
        }
    }
}
