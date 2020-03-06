using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Rooms;

namespace Wanderer.Actions
{
    public class PickUpFrame : Frame
    {
        public IItem Item { get; set;}
        public IRoom FromRoom { get; set;}

        public PickUpFrame(IActor performedBy,IAction action,IItem item,IRoom fromRoom,double attitude):base(performedBy,action,attitude)
        {
            Item = item;
            FromRoom = fromRoom;
        }
    }
}