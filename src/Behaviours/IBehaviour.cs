using System;
using System.Text;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Rooms;

namespace Wanderer.Behaviours
{
    public interface IBehaviour : IHasStats
    {
        IHasStats Owner { get; set; }

        /// <summary>
        /// Called after all targeting decisions have been made for an action but before it has
        /// been resolved.  The action may yet be cancelled (see <see cref="OnPop"/>)
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        void OnPush(IWorld world, IUserinterface ui, ActionStack stack, Frame frame);


        /// <summary>
        /// Called after an action <paramref name="frame"/> is popped off the stack and resolved
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        void OnPop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame);

        void OnRoundEnding(IWorld world,IUserinterface ui,Guid round);

        /// <summary>
        /// Called when any actor enters a <paramref name="room"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="ui"></param>
        /// <param name="round"></param>
        /// <param name="actor"></param>
        /// <param name="room"></param>
        void OnEnter(IWorld world,IUserinterface ui,Guid round, IActor actor, IRoom room);
    }
}
