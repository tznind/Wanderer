using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Places
{
    public class Room : IPlace
    {
        public HashSet<IActor> Occupants { get; set; } =  new HashSet<IActor>();
        public string Title { get; set; }

        public IList<IAction> BaseActions { get; set; } = new List<IAction>();
        public Dictionary<Direction, IPlace> Adjoining { get; set; } = new Dictionary<Direction, IPlace>();

        public void AddAction(IAction action)
        {
            BaseActions.Add(action);
        }

        public IList<IAction> GetActions()
        {
            var toReturn = new List<IAction>();
            toReturn.AddRange(BaseActions);
            return toReturn;
        }

        public void AddActor(IActor actor)
        {
            Occupants.Add(actor);
        }
    }
}