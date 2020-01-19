using StarshipWanderer.Actors;
using StarshipWanderer.Items;

namespace StarshipWanderer.Actions
{
    public class GiveFrame : Frame
    {
        public IItem ItemToGive { get; }

        public GiveFrame(IActor actor, GiveAction giveAction, IItem toGive, IActor toGiveTo,double attitude):base(actor,giveAction,attitude)
        {
            ItemToGive = toGive;
            TargetIfAny = toGiveTo;
        }
    }
}