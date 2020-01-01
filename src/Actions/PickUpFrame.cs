using StarshipWanderer.Actors;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Actions
{
    public class PickUpFrame : Frame
    {
        public IItem Item { get; }
        public IPlace FromPlace { get; }

        public PickUpFrame(IActor performedBy,IAction action,IItem item,IPlace fromPlace):base(performedBy,action)
        {
            Item = item;
            FromPlace = fromPlace;
        }
    }
}