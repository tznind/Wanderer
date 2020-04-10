using System.Collections.Generic;
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
            HotKey = 'l';
        }
        
        public override void Push(IWorld world,IUserinterface ui, ActionStack stack, IActor actor)
        {
            //ask actor to pick a direction
            if (actor.Decide<Direction>(ui, "LeaveAction Direction", null, out var direction,GetLeaveDirections(actor), 0))
                stack.Push(new LeaveFrame(actor,this,direction,0));
        }

        private Direction[] GetLeaveDirections(IActor performer)
        {
            return performer.CurrentLocation.LeaveDirections.ToArray();
        }

        protected override void PopImpl(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (LeaveFrame) frame;

            if(f.LeaveDirection == Direction.None)
                return;
            
            var oldPoint = frame.PerformedBy.CurrentLocation.GetPoint();
            var newPoint = oldPoint.Offset(f.LeaveDirection,1);
            
            IRoom goingTo;
            
            List<IBehaviour> newRoomBehaviours = new List<IBehaviour>();

            if (world.Map.ContainsKey(newPoint))
                goingTo = world.Map[newPoint];
            else
            {
                var newRoom = world.GetNewRoom(newPoint);
                
                //however you got into this room you should be able to get back again
                newRoom.LeaveDirections.Add(f.LeaveDirection.Opposite());

                world.Map.Add(newPoint,newRoom);
                goingTo = newRoom;

                newRoomBehaviours.AddRange(newRoom.GetFinalBehaviours(frame.PerformedBy));
            }

            frame.PerformedBy.Move(goingTo);

            ui.Log.Info(new LogEntry($"{frame.PerformedBy} moved {f.LeaveDirection} to {goingTo}",stack.Round,oldPoint));

            //fire all on enter events
            foreach (var behaviour in 
                stack.Behaviours.Union(newRoomBehaviours).ToArray())
                behaviour.OnEnter(world, ui, stack.Round, frame.PerformedBy, goingTo);
        }

        public override bool HasTargets(IActor performer)
        {
            return GetLeaveDirections(performer).Any();
        }

        public override IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            yield break;
        }

        public override string ToString()
        {
            if(Owner is IActor)
                return $"{Name}";

            //leave via item? what is this a teleporter?
            return base.ToString();
        }
    }
}