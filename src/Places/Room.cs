using System.Collections.Generic;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace StarshipWanderer
{
    public class Room : IPlace
    {
        public HashSet<IActor> Occupants { get; } =  new HashSet<IActor>();
        public string Title { get; set; }

        private readonly IList<IAction> _actions = new List<IAction>();
        public Dictionary<Direction, IPlace> Adjoining { get; } = new Dictionary<Direction, IPlace>(); 

        public Room(IWorld world)
        {
            //the player can leave this room
            _actions.Add(new Leave(world,world.Player));
        }

        public void AddAction(IAction action)
        {
            _actions.Add(action);
        }

        public IList<IAction> GetActions()
        {
            var toReturn = new List<IAction>();
            toReturn.AddRange(_actions);
            return toReturn;
        }

        public void AddActor(IActor actor)
        {
            Occupants.Add(actor);
        }
    }
}