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
            
            base.Push(ui,stack);
        }

        public override void Pop(IUserinterface ui, ActionStack stack)
        {
            if(Direction == Direction.None)
                return;

            var oldRoom = World.CurrentLocation;
            var oldPoint = World.Map.GetPoint(oldRoom);
            var newPoint = oldPoint.Offset(Direction,1);

            if (World.Map.ContainsKey(newPoint))
                World.CurrentLocation = World.Map[newPoint];
            else
            {
                var newRoom = World.RoomFactory.Create(World);
                World.Map.Add(newPoint,newRoom);
                World.CurrentLocation = newRoom;
            }
            
            ui.Refresh();
        }
    }
}