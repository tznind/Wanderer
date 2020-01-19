using StarshipWanderer.Actors;
using StarshipWanderer.Items;

namespace StarshipWanderer.Actions
{
    public class DropFrame : Frame
    {
        public IItem ToDrop { get; set; }

        public DropFrame(IActor actor, IAction action, IItem toDrop,double attitude):base(actor,action,attitude)
        {
            ToDrop = toDrop;
        }
    }
}