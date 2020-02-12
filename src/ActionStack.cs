using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Behaviours;

namespace Wanderer
{
    /// <summary>
    /// Handles resolving <see cref="IAction"/> and response actions triggered (e.g. by <see cref="IBehaviour"/>)
    /// </summary>
    public class ActionStack : Stack<Frame>
    {
        public Guid Round { get; }= Guid.NewGuid();

        /// <summary>
        /// Runs the <paramref name="firstAction"/> and evaluates all responders
        /// </summary>
        /// <param name="ui">When decisions require user input, this handles it</param>
        /// <param name="firstAction">The initial action (to go on bottom of stack)</param>
        /// <param name="performer">Who is attempting <paramref name="firstAction"/></param>
        /// <param name="responders">All valid responders</param>
        public bool RunStack(IUserinterface ui, IAction firstAction,IActor performer, IEnumerable<IBehaviour> responders)
        {
            responders = responders?.ToArray() ?? new IBehaviour[0];

            //and run push event on the action
            firstAction.Push(ui,this,performer);

            //If the action decided not to push after all (e.g. UI cancel or decision not to push)
            if (this.Count == 0)
                return false; //initial action was aborted

            //check all behaviours to see if they want to respond (by pushing actions etc)
            foreach (IBehaviour responder in responders.ToArray())  //ToArray needed because they can self destruct at this time!
                responder.OnPush(ui, this,Peek());
            
            //run all tasks that are not pending cancellation
            while(TryPop(out Frame current))
                if (!current.Cancelled)
                {
                    current.Action.Pop(ui,this, current);

                    foreach (IBehaviour responder in responders.ToArray())
                        responder.OnPop(ui, this,current);
                }
                    
            
            Clear();

            return true;
        }
    }
}