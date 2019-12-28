using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Targets;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Actions
{
    class FightAction : Action
    {
        public IActor Target { get; set; }

        public FightAction(IWorld world, IActor performedBy) : base(world, performedBy)
        {
        }


        public override void Push(IUserinterface ui, ActionStack stack)
        {
            var targets = World.CurrentLocation.Occupants.Where(a => !(a is You)).ToArray();

            if (!targets.Any())
            {
                ui.ShowMessage("No Targets","There is nobody else to Fight", false);
                return;
            }

            if (PerformedBy.Decide(ui,"Fight", null, out IActor toFight, targets,-20))
            {
                Target = toFight;
                base.Push(ui, stack);
            }
        }

        public override void Pop(IUserinterface ui, ActionStack stack)
        {
            ui.ShowMessage("Fight Results",$"{PerformedBy} fought and killed {Target}",true);
            World.CurrentLocation.Occupants.Remove(Target);

        }
    }
}
