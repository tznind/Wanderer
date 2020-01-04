using StarshipWanderer.Actors;
using StarshipWanderer.Items;

namespace StarshipWanderer.Actions
{
    public class DropFrame : Frame
    {
        public IItem ToDrop { get; set; }

        public DropFrame(IActor actor, IAction action, IItem toDrop):base(actor,action)
        {
            ToDrop = toDrop;
        }
    }
}