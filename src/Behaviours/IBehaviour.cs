using System;
using System.Text;
using StarshipWanderer.Actions;

namespace StarshipWanderer.Behaviours
{
    public interface IBehaviour : IAreIdentical<IBehaviour>
    {
        IHasStats Owner { get; set; }

        /// <summary>
        /// Called after all targeting decisions have been made for an action but before it has
        /// been resolved.  The action may yet be cancelled (see <see cref="OnPop"/>)
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        void OnPush(IUserinterface ui, ActionStack stack,Frame frame);


        /// <summary>
        /// Called after an action <paramref name="frame"/> is popped off the stack and resolved
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="stack"></param>
        /// <param name="frame"></param>
        void OnPop(IUserinterface ui, ActionStack stack, Frame frame);

        void OnRoundEnding(IUserinterface ui,Guid round);
    }
}
