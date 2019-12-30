using System;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;

namespace StarshipWanderer.Actions
{
    public class Leave : Action
    {
        public Direction Direction;

        public Leave(IActor performedBy):base(performedBy)
        {
        }
        
        public override void Push(IUserinterface ui, ActionStack stack)
        {
            //ask actor to pick a direction
            if (PerformedBy.Decide<Direction>(ui, "Leave Direction", null, out Direction,
                Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(d=>d!=Direction.None).ToArray(), 0))
                base.Push(ui,stack);
        }

        public override void Pop(IUserinterface ui, ActionStack stack)
        {
            if(Direction == Direction.None)
                return;
            
            var oldPoint = PerformedBy.CurrentLocation.GetPoint();
            var newPoint = oldPoint.Offset(Direction,1);
            
            IPlace goingTo;

            var world = PerformedBy.CurrentLocation.World;

            if (world.Map.ContainsKey(newPoint))
                goingTo = world.Map[newPoint];
            else
            {
                var newRoom = world.RoomFactory.Create(world);
                world.Map.Add(newPoint,newRoom);
                goingTo = newRoom;
            }

            PerformedBy.Move(goingTo);

            ui.Log.Info($"{PerformedBy} moved to {goingTo}");

            //since they should now be in the new location we should prep the command for reuse
            ui.Refresh();

        }
    }
}