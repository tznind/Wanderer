using System.Linq;
using Wanderer.Behaviours;

namespace Wanderer.Items
{
    public class SingleUseBehaviour : Behaviour
    {
        public SingleUseBehaviour(IHasStats owner) : base(owner)
        {
        }
        public override void OnPop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var item = ((SingleUse) Owner).OwnerItem;

            //if an action is pushed onto the stack that comes from the owned item
            if (item.GetFinalActions(frame.PerformedBy).Any(a=>ReferenceEquals(a,frame.Action)))
            {
                //stack of 2+ just decrements the stack
                if(item is IItemStack s && s.StackSize > 1)
                {
                    s.StackSize --;
                    return;
                }
                
                //erase it from existence
                frame.PerformedBy.CurrentLocation.World.Erase(item);
            }
        }
    }
}