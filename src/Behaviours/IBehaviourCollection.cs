using System.Collections.Generic;
using StarshipWanderer.Adjectives;

namespace StarshipWanderer.Behaviours
{
    public interface IBehaviourCollection: IList<IBehaviour>, ISwCollection<IBehaviour>
    {
    }
}