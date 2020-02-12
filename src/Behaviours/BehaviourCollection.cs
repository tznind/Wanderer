using System.Collections.Generic;
using Wanderer.Adjectives;

namespace Wanderer.Behaviours
{
    class BehaviourCollection : SwCollection<IBehaviour>,IBehaviourCollection
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