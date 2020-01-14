using System.Collections.Generic;
using StarshipWanderer.Adjectives;

namespace StarshipWanderer.Actions
{
    public class ActionCollection : SwCollection<IAction>, IActionCollection
    {
        public ActionCollection()
        {
            
        }
        public ActionCollection(IEnumerable<IAction> actions)
        {
            AddRange(actions);
        }

    }
}