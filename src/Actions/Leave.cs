using System;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;

namespace StarshipWanderer.Actions
{
    public class Leave : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack, IActor actor)
        {
            //ask actor to pick a direction
            if (actor.Decide<Direction>(ui, "Leave Direction", null, out var direction,actor.CurrentLocation.LeaveDirections.ToArray(), Attitude))
                stack.Push(new LeaveFrame(actor,this,direction));
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (LeaveFrame) frame;

            if(f.Direction == Direction.None)
                return;
            
            var oldPoint = frame.PerformedBy.CurrentLocation.GetPoint();
            var newPoint = oldPoint.Offset(f.Direction,1);
            
            IPlace goingTo;

            var world = frame.PerformedBy.CurrentLocation.World;

            if (world.Map.ContainsKey(newPoint))
                goingTo = world.Map[newPoint];
            else
            {
                var newRoom = world.RoomFactory.Create(world);
                
                //however you got into this room you should be able to get back again
                newRoom.LeaveDirections.Add(f.Direction.Opposite());

                world.Map.Add(newPoint,newRoom);
                goingTo = newRoom;
            }

            frame.PerformedBy.Move(goingTo);

            ui.Log.Info(new LogEntry($"{frame.PerformedBy} moved to {goingTo}",stack.Round,oldPoint));
        }
    }
}