using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Targets;
using StarshipWanderer.Actors;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actions
{
    class FightAction : Action
    {
        public IActor Target { get; set; }

        public FightAction(IActor performedBy) : base(performedBy)
        {
        }


        public override void Push(IUserinterface ui, ActionStack stack)
        {
            var targets = PerformedBy.CurrentLocation.World.Population
                .Where(a => a.CurrentLocation == PerformedBy.CurrentLocation && !(a is You)).ToArray();

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

            if (Target.GetFinalStats()[Stat.Fight] > PerformedBy.GetFinalStats()[Stat.Fight])
            {
                ui.ShowMessage("Fight Results",$"{PerformedBy} fought {Target} but was killed",true);
                PerformedBy.Kill(ui);
            }
            else
            {
                ui.ShowMessage("Fight Results",$"{PerformedBy} fought and killed {Target}",true);
                Target.Kill(ui);
            }

        }
    }
}
