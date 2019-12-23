namespace StarshipWanderer.Actions
{
    public class Leave : Action
    {
        public Leave(IWorld world):base(world)
        {
            
        }

        public override void Perform(IUserinterface ui)
        {
            var dir = ui.GetOption<Direction>("Leave Direction");
            
            if(dir == Direction.None)
                return;
            
            var oldRoom = World.CurrentLocation;

            if (oldRoom.Adjoining.ContainsKey(dir))
                World.CurrentLocation = oldRoom.Adjoining[dir];
            else
            {
                var newRoom = World.RoomFactory.Create(World);
                newRoom.Adjoining.Add(dir.Opposite(),oldRoom);
                World.CurrentLocation = newRoom;
            }
            
            ui.Refresh();
        }
    }
}