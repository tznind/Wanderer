using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer.Actions
{
    public class Leave : Action
    {
        public Direction Direction;

        public Leave(IWorld world,IActor performedBy):base(world,performedBy)
        {
            
        }
        
        public override void Push(IUserinterface ui, ActionStack stack)
        {
            Direction = ui.GetOption<Direction>("Leave Direction");
            
            if(Direction == Direction.None)
                return;
            
            stack.Push(this);
        }

        public override void Pop(IUserinterface ui, ActionStack stack)
        {
            if(Direction == Direction.None)
                return;

            var oldRoom = World.CurrentLocation;

            if (oldRoom.Adjoining.ContainsKey(Direction))
                World.CurrentLocation = oldRoom.Adjoining[Direction];
            else
            {
                var newRoom = World.RoomFactory.Create(World);
                newRoom.Adjoining.Add(Direction.Opposite(),oldRoom);
                World.CurrentLocation = newRoom;
            }
            
            ui.Refresh();
        }
    }
}