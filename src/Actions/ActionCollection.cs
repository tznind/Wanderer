using System.Collections.Generic;
using Wanderer.Adjectives;

namespace Wanderer.Actions
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