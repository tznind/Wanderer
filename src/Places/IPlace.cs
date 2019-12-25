using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Places
{
    public interface IPlace
    {
        string Title { get; set; }

        /// <summary>
        /// Adds a new action which can be performed in the room
        /// </summary>
        /// <param name="action"></param>
        void AddAction(IAction action);

        void AddActor(IActor actor);

        HashSet<IActor> Occupants { get; set; }
        IList<IAction> GetActions();

        Dictionary<Direction,IPlace> Adjoining { get; set; }
    }
}