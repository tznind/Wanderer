using System;
using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer
{
    public class ActionStack : Stack<Frame>
    {
        public void RunStack(IUserinterface ui, IAction firstAction,IActor performer, IEnumerable<IBehaviour> responders)
        {
            //and run push event on the action
            firstAction.Push(ui,this,performer);

            //check all behaviours to see if they want to respond (by pushing actions etc)
            foreach (IBehaviour responder in responders) 
                responder.OnPush(ui, this,Peek());

            var pending = new Queue<Frame>();

            //run all tasks that are not pending cancellation
            while(TryPop(out Frame current))
                switch (current.Cancelled)
                {
                    case CancellationStatus.NotCancelled:
                        current.Action.Pop(ui,this, current);
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
            while(pending.TryDequeue(out Frame current))
                if(current.Cancelled != CancellationStatus.Cancelled)
                    current.Action.Pop(ui,this, current);

            Clear();
        }
    }

    public class Frame
    {
        public IActor PerformedBy { get; set; }
        public IAction Action { get; set; }
        public CancellationStatus Cancelled { get; set; } = CancellationStatus.NotCancelled;

        public Frame(IActor performedBy, IAction action)
        {
            PerformedBy = performedBy;
            Action = action;
        }

    }

}