using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Places;

namespace Wanderer.Actions
{
    public class PickUpFrame : Frame
    {
        public IItem Item { get; }
        public IPlace FromPlace { get; }

        public PickUpFrame(IActor performedBy,IAction action,IItem item,IPlace fromPlace,double attitude):base(performedBy,action,attitude)
        {
            Item = item;
            FromPlace = fromPlace;
        }
    }
}