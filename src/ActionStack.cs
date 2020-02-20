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
        /// Runs the <paramref name="firstAction"/> and evaluates all responders.
        /// This overload shortcuts decision making and allows arbitrary action frames
        /// to run even when the <paramref name="performer"/> could not normally pick them
        /// (so be careful using it).
        /// </summary>
        /// <param name="world">Where the action is happening</param>
        /// <param name="ui">When decisions require user input, this handles it</param>
        /// <param name="firstAction">The initial action (to go on bottom of stack)</param>
        /// <param name="performer">Who is attempting <paramref name="firstAction"/></param>
        /// <param name="responders">All valid responders</param>
        public bool RunStack(IWorld world,IUserinterface ui, IAction firstAction,IActor performer, IEnumerable<IBehaviour> responders)
        {
            //and run push event on the action
            firstAction.Push(world,ui,this,performer);

            return RunStack(world, ui, performer, responders);
        }
        
        /// <summary>
        /// Runs the <paramref name="frameToRun"/> and evaluates all responders
        /// </summary>
        /// <param name="world">Where the action is happening</param>
        /// <param name="ui">When decisions require user input, this handles it</param>
        /// <param name="frameToRun">The initial action frame (to go on bottom of stack)</param>
        /// <param name="performer">Who is attempting <paramref name="frameToRun"/></param>
        /// <param name="responders">All valid responders</param>
        public bool RunStack(IWorld world,IUserinterface ui, Frame frameToRun,IActor performer, IEnumerable<IBehaviour> responders)
        {
            Push(frameToRun);

            return RunStack(world, ui, performer, responders);
        }

        private bool RunStack(IWorld world,IUserinterface ui,IActor performer, IEnumerable<IBehaviour> responders)
        {
            responders = responders?.ToArray() ?? new IBehaviour[0];
            
            //If the action decided not to push after all (e.g. UI cancel or decision not to push)
            if (this.Count == 0)
                return false; //initial action was aborted

            //check all behaviours to see if they want to respond (by pushing actions etc)
            foreach (IBehaviour responder in responders.ToArray())  //ToArray needed because they can self destruct at this time!
                responder.OnPush(world,ui, this,Peek());
            
            //run all tasks that are not pending cancellation
            while(TryPop(out Frame current))
                if (!current.Cancelled)
                {
                    current.Action.Pop(world,ui,this, current);

                    foreach (IBehaviour responder in responders.ToArray())
                        responder.OnPop(world,ui, this,current);
                }
            
            Clear();

            return true;
        }
    }
}