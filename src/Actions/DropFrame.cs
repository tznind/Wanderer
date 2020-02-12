using Wanderer.Actors;
using Wanderer.Items;

namespace Wanderer.Actions
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