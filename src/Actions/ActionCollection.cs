using System;
using System.Collections.Generic;
using System.Linq;
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

        public IActionCollection Clone()
        {
            return new ActionCollection(this.Select(a=>a.Clone()));
        }
    }
}