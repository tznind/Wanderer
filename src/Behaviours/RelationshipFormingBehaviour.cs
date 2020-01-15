using System.Linq;
using System.Threading;
using StarshipWanderer.Actions;
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
            
            if(frame.TargetIfAny == null)            
                return;

            //if the target is me then I might get angry
            if (frame.TargetIfAny == Owner)
                IDoCare(ui,world,frame,stack,frame.Action.Attitude);
            else
            {
                //is the target one of my friends?
                double howMuchICare = world.Relationships.Where(r => r.AppliesTo((IActor) Owner, frame.TargetIfAny))
                    .Sum(r => r.Attitude);

                //Your hurting one of my friends, im unhappy
                if (howMuchICare > 0.0001)
                    IDoCare(ui,world,frame,stack,frame.Action.Attitude);

                //your hurting my enemies, I'm happy!
                if(howMuchICare < 0.001)
                    IDoCare(ui,world,frame,stack,-0.5 * frame.Action.Attitude);
            }
        }

        private void IDoCare(IUserinterface ui, IWorld world, Frame frame, ActionStack stack, double attitudeChange)
        {
            var o = (IActor) Owner;
            //but do I know
            
            //apply relationship change where I change towards the action initiator
            if(o.IsAwareOf(frame.PerformedBy))
                world.Relationships.Apply(new SystemArgs(ui,attitudeChange,
                frame.PerformedBy,
                o,stack.Round));
        }
    }
}