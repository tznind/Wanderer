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


        public bool HasAction(Type t)
        {
            return GetAction(t) != null;
        }
        public bool HasAction(string typename)
        {
            return GetAction(typename) != null;
        }

        public IAction GetAction(Type t)
        {
            return this.FirstOrDefault(a=>a.GetType() == t);
        }
        public IAction GetAction(string typename)
        {
            string withSuffix = typename + "Action";

            return this.FirstOrDefault(a=>
                        a.GetType().Name == typename ||
                        a.GetType().Name == withSuffix
            );
        }

    }
}