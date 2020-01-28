using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Actions
{
    public class FightAction : Action
    {
        public override void Push(IUserinterface ui, ActionStack stack,IActor actor)
        {
            const int fightAttitude = -20;

            if (actor.Decide(ui,"Fight", null, out IActor toFight, GetTargets(actor),fightAttitude)) 
                stack.Push(new FightFrame(actor, toFight, this,actor.CurrentLocation.World.InjurySystems.First(),fightAttitude));
        }

        public override void Pop(IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (FightFrame) frame;
            
            var attackerAdvantage = f.PerformedBy.GetFinalStats()[Stat.Fight] - f.TargetIfAny.GetFinalStats()[Stat.Fight];
            
            ui.Log.Info(new LogEntry($"{f.PerformedBy} fought {f.TargetIfAny}",stack.Round,f.PerformedBy));

            //inflict damage on the target
            f.InjurySystem.Apply(new SystemArgs(ui,
                attackerAdvantage + 20, //Even fighting someone of lower Fight results in injury so add 20

                f.PerformedBy,
                f.TargetIfAny,
                stack.Round));

            //inflict damage back again
            f.InjurySystem.Apply(new SystemArgs(ui,
                -attackerAdvantage + 20,
                f.TargetIfAny,
                f.PerformedBy,
                stack.Round));
            
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }

        private IActor[] GetTargets(IActor performer)
        {
            return performer.GetCurrentLocationSiblings();
        }
    }
}
