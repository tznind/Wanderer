using StarshipWanderer.Actors;
using StarshipWanderer.Items;

namespace StarshipWanderer.Actions
{
    public class ReadFrame : Frame
    {
        public IItem Item { get; set; }

        public ReadFrame(IActor performedBy, IAction action, double attitude, IItem item) : base(performedBy, action, attitude)
        {
            Item = item;
        }
    }
}