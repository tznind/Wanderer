using System.Collections.Generic;
using StarshipWanderer.Adjectives;

namespace StarshipWanderer.Behaviours
{
    class BehaviourCollection : Collection<IBehaviour>,IBehaviourCollection
    {
        public BehaviourCollection()
        {
            
        }
        public BehaviourCollection(IEnumerable<IBehaviour> behaviours)
        {
            AddRange(behaviours);
        }
    }
}