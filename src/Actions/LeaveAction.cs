using System;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Rooms;

namespace Wanderer.Actions
{
    public class LeaveAction : Action
    {
        
        public LeaveAction(IHasStats owner) : base(owner)
        {
        }

        private LeaveAction() : base(null)
        {
        }
        public override char HotKey => 'l';
        
        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            //ask actor to pick a direction
            if (actor.Decide<Direction>(ui, "LeaveAction Direction", null, out var direction,GetTargets(actor), 0))
                stack.Push(new LeaveFrame(actor,this,direction,0));
        }

        private Direction[] GetTargets(IActor performer)
        {
            return performer.CurrentLocation.LeaveDirections.ToArray();
        }

        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (LeaveFrame) frame;

            if(f.LeaveDirection == Direction.None)
                return;
            
            var oldPoint = frame.PerformedBy.CurrentLocation.GetPoint();
            var newPoint = oldPoint.Offset(f.LeaveDirection,1);
            
            IRoom goingTo;

            if (world.Map.ContainsKey(newPoint))
                goingTo = world.Map[newPoint];
            else
            {
                var newRoom = world.GetNewRoom(newPoint);
                
                //however you got into this room you should be able to get back again
                newRoom.LeaveDirections.Add(f.LeaveDirection.Opposite());

                world.Map.Add(newPoint,newRoom);
                goingTo = newRoom;
            }

            frame.PerformedBy.Move(goingTo);

            ui.Log.Info(new LogEntry($"{frame.PerformedBy} moved {f.LeaveDirection} to {goingTo}",stack.Round,oldPoint));
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }

    }
}