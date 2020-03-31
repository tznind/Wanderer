using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Items;
using Wanderer.Stats;

namespace Wanderer.Actions
{
    public class DropAction : Action
    {
        public DropAction(IHasStats owner) : base(owner)
        {
            HotKey = 'd';
            Attitude = -10;
        }

        public override void Push(IWorld world, IUserinterface ui, ActionStack stack, IActor actor)
        {
            stack.Push(new Frame(actor,this,GetAttitude(actor,Owner) ?? Attitude){TargetIfAny = Owner});
        }

        protected override void PopImpl(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = frame;

            //if we still have the item we should drop it
            if(f.PerformedBy.Items.Contains(f.TargetIfAny))
                ((IItem)f.TargetIfAny).Drop(ui, f.PerformedBy,stack.Round);
        }

        public override IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            return new []{Owner};
        }
        
        protected override double? GetAttitude(IActor performer, IHasStats target)
        {
            //value of item is total value of the item to the recipient
            return target.GetFinalStats(performer)[Stat.Value];
        }

    }
}
