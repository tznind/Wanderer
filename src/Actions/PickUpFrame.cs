using StarshipWanderer.Actors;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Actions
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