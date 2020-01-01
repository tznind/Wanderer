using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Targets;
using StarshipWanderer.Actors;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actions
{
    public class FightAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack,IActor actor)
        {
            var targets = actor.CurrentLocation.World.Population
                .Where(a => a.CurrentLocation == actor.CurrentLocation && !(a is You)).ToArray();

            if (!targets.Any() && actor is You)
            {
                //only complain about lack of targets if the player is attempting the action
                ui.ShowMessage("No Targets","There is nobody else to Fight", false,stack.Round);
                return;
            }

            if (actor.Decide(ui,"Fight", null, out IActor toFight, targets,-20))
            {
                stack.Push(new FightFrame(actor,toFight,this));
            }
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (FightFrame) frame;

            if (f.FightTarget.GetFinalStats()[Stat.Fight] > f.PerformedBy.GetFinalStats()[Stat.Fight])
            {
                ui.Log.Info($"{f.PerformedBy} fought {f.FightTarget} but was killed",stack.Round);
                f.PerformedBy.Kill(ui);
            }
            else
            {
                ui.Log.Info($"{f.PerformedBy} fought and killed {f.FightTarget}",stack.Round);
                f.FightTarget.Kill(ui);
            }

        }
    }
}
