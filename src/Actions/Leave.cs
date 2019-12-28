using System;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Places;

namespace StarshipWanderer.Actions
{
    public class Leave : Action
    {
        private IPlace _leavingFrom;
        public Direction Direction;

        public Leave(IWorld world,IActor performedBy,IPlace leavingFrom):base(world,performedBy)
        {
            _leavingFrom = leavingFrom;
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

            if(!_leavingFrom.Occupants.Contains(PerformedBy))
                throw new Exception("Actor was not in the room they were trying to leave");
            
            var oldPoint = World.Map.GetPoint(_leavingFrom);
            var newPoint = oldPoint.Offset(Direction,1);
            
            IPlace goingTo;

            if (World.Map.ContainsKey(newPoint))
                goingTo = World.Map[newPoint];
            else
            {
                var newRoom = World.RoomFactory.Create(World);
                World.Map.Add(newPoint,newRoom);
                goingTo = newRoom;
            }

            PerformedBy.Move(World,_leavingFrom, goingTo);
            
            //since they should now be in the new location we should prep the command for reuse
            _leavingFrom = goingTo;
            ui.Refresh();
        }
    }
}