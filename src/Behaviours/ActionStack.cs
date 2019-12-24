using System.Collections.Generic;
using StarshipWanderer.Actions;

namespace StarshipWanderer.Behaviours
{
    public class ActionStack : Stack<IAction>
    {
        public void Remove(IAction toRemove)
        {
            List<IAction> popped = new List<IAction>();

            while (TryPop(out IAction p))
            {
                if(p != toRemove)
                    popped.Add(p);
            }

            foreach (IAction p in popped)
                Push(p);
        }
    }
}