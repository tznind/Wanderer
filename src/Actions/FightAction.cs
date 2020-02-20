using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Items;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Actions
{
    public class FightAction : Action
    {
        public override void Push(IWorld world,IUserinterface ui, ActionStack stack,IActor actor)
        {
            const int fightAttitude = -20;

            if (actor.Decide(ui,"Fight", null, out IActor toFight, GetTargets(actor),fightAttitude)) 
                stack.Push(new FightFrame(actor, toFight, this,actor.CurrentLocation.World.InjurySystems.First(),fightAttitude));
        }

        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (FightFrame) frame;
            
            var attackerAdvantage = f.PerformedBy.GetFinalStats()[Stat.Fight] - f.TargetIfAny.GetFinalStats()[Stat.Fight];
            
            ui.Log.Info(new LogEntry($"{f.PerformedBy} fought {f.TargetIfAny}",stack.Round,f.PerformedBy));
            
            //inflict damage on the target
            f.InjurySystem.Apply(new SystemArgs(world,ui,
                attackerAdvantage + 20, //Even fighting someone of lower Fight results in injury so add 20

                f.PerformedBy,
                f.TargetIfAny,
                stack.Round));

            //inflict damage back again
            f.InjurySystem.Apply(new SystemArgs(world,ui,
                -attackerAdvantage + 20,
                f.TargetIfAny,
                f.PerformedBy,
                stack.Round));

            foreach (var a in new []{f.TargetIfAny, f.PerformedBy})
            {
                //fighting makes you tired
                a.Adjectives.Add(
                    //expires at the end of the next round
                    new Tired(a).WithExpiry(2));
            }
            
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }

        private IActor[] GetTargets(IActor performer)
        {
            return performer.GetCurrentLocationSiblings(false);
        }
    }
}
