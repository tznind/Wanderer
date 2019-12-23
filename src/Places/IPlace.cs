using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Places
{
    public interface IPlace
    {
        string Title { get; }

        void AddAction(IAction action);

        List<IActor> Occupants { get; }
        IList<IAction> GetActions();

        Dictionary<Direction,IPlace> Adjoining { get; }
    }
}