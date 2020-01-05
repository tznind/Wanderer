using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Targets;
using StarshipWanderer.Actors;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Actions
{
    public class FightAction : Action
    {
        public FightAction()
        {
            Attitude = -20;
        }

        public override void Push(IUserinterface ui, ActionStack stack,IActor actor)
        {
            if (actor.Decide(ui,"Fight", null, out IActor toFight, actor.GetCurrentLocationSiblings(),Attitude))
                stack.Push(new FightFrame(actor, toFight, this,new InjurySystem()));
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (FightFrame) frame;

            //inflict damage on the target
            f.InjurySystem.Apply(new SystemArgs(ui,
                f.PerformedBy.GetFinalStats()[Stat.Fight] - f.FightTarget.GetFinalStats()[Stat.Fight],
                f.PerformedBy,
                f.FightTarget,
                stack.Round));

            //inflict damage back again
            f.InjurySystem.Apply(new SystemArgs(ui,
                f.FightTarget.GetFinalStats()[Stat.Fight] - f.PerformedBy.GetFinalStats()[Stat.Fight],
                f.FightTarget,
                f.PerformedBy,
                stack.Round));
            
            ui.Log.Info($"{f.PerformedBy} fought {f.FightTarget}",stack.Round);
        }
    }
}
