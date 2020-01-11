using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Relationships;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Behaviours
{
    public class RelationshipFormingBehaviour : Behaviour
    {
        public RelationshipFormingBehaviour(IActor owner) : base(owner)
        {
        }

        public override void OnPush(IUserinterface ui, ActionStack stack, Frame frame)
        {
            base.OnPush(ui, stack, frame);

            var world = frame.PerformedBy.CurrentLocation.World;
            
            //if the target is me then I might get angry
            if(frame.TargetIfAny == Owner)
                world.Relationships.Apply(new SystemArgs(ui,frame.Action.Attitude,frame.PerformedBy,frame.TargetIfAny,stack.Round));
        }
    }
}