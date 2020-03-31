using System.Linq;
using System.Threading;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Relationships;
using Wanderer.Systems;

namespace Wanderer.Behaviours
{
    public class RelationshipFormingBehaviour : Behaviour
    {
        public RelationshipFormingBehaviour(IActor owner) : base(owner)
        {
        }

        public override void OnPop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            base.OnPop(world,ui, stack, frame);
            
            if(! (frame.TargetIfAny is IActor))
                return;

            //if the target is me then I might get angry
            if (frame.TargetIfAny == Owner)
                IDoCare(world,ui,frame,stack,frame.Attitude);
            else
            {
                //is the target one of my friends?
                double howMuchICare = world.Relationships.Where(r => r.AppliesTo((IActor) Owner, (IActor)frame.TargetIfAny))
                    .Sum(r => r.Attitude);

                //Your hurting one of my friends, im unhappy
                if (howMuchICare > 0.0001)
                    IDoCare(world,ui,frame,stack,frame.Attitude);

                //your hurting my enemies, I'm happy!
                if(howMuchICare < 0.001)
                    IDoCare(world,ui,frame,stack,-0.5 * frame.Attitude);
            }
        }

        private void IDoCare(IWorld world, IUserinterface ui, Frame frame, ActionStack stack, double attitudeChange)
        {
            var o = (IActor) Owner;
            //but do I know
            
            //apply relationship change where I change towards the action initiator
            if(o.IsAwareOf(frame.PerformedBy))
                world.Relationships.Apply(new SystemArgs(world,ui,attitudeChange,
                frame.PerformedBy,
                o,stack.Round));
        }
    }
}