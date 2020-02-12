using System.Collections.Generic;
using Wanderer.Adjectives;

namespace Wanderer.Behaviours
{
    public interface IBehaviourCollection: IList<IBehaviour>, ISwCollection<IBehaviour>
    {
    }
}