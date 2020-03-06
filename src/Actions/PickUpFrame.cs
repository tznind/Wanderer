using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Places;

namespace Wanderer.Actions
{
    public class PickUpFrame : Frame
    {
        public IItem Item { get; set;}
        public IRoom FromPlace { get; set;}

        public PickUpFrame(IActor performedBy,IAction action,IItem item,IRoom fromPlace,double attitude):base(performedBy,action,attitude)
        {
            Item = item;
            FromPlace = fromPlace;
        }
    }
}