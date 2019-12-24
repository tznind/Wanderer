using System;
using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer
{
    public class ActionStack : Stack<IAction>
    {
        public void RunStack(IUserinterface ui, IAction firstAction, IEnumerable<IBehaviour> responders)
        {
            //push the action onto stack
            Push(firstAction);

            //and run push event on the action
            firstAction.Push(ui,this);

            //check all behaviours to see if they want to respond (by pushing actions etc)
            foreach (IBehaviour responder in responders) 
                responder.OnPush(ui, this);

            var pending = new Queue<IAction>();

            //run all tasks that are not pending cancellation
            while(TryPop(out IAction current))
                switch (current.Cancelled)
                {
                    case CancellationStatus.NotCancelled:
                        current.Pop(ui,this);
                        break;
                    case CancellationStatus.CancellationPending:
                        pending.Enqueue(current);
                        break;
                    case CancellationStatus.Cancelled:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            //now run any actions we thought were going to be cancelled but weren't after all
            while(pending.TryDequeue(out IAction current))
                if(current.Cancelled != CancellationStatus.Cancelled)
                    current.Pop(ui,this);

            Clear();
        }
    }
}