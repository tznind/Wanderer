using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Specifies a specific kind of damage inflicted when performing this
        /// fight action (e.g. for a flame weapon).  This overrides the
        /// <see cref="IActor.InjurySystem"/>
        /// </summary>
        public IInjurySystem InjurySystem { get; set; }

        public FightAction(IHasStats owner):base(owner)
        {
            HotKey = 'f';
        }
        
        public IActor PrimeWithTarget {get;set;}

        public override void Push(IWorld world,IUserinterface ui, ActionStack stack,IActor actor)
        {
            const int fightAttitude = -20;

            IActor toFight = PrimeWithTarget;

            //explicit injury system of this fight action or your current best
            var system = InjurySystem ?? actor.GetBestInjurySystem();

            //does the world support injuries
            if(system != null)
                if (toFight != null || actor.Decide(ui,"Fight", null, out toFight, GetTargets(actor).Cast<IActor>().ToArray(),fightAttitude)) 
                    stack.Push(new FightFrame(actor, toFight, this,system,fightAttitude));
        }

        public override void Pop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            var f = (FightFrame) frame;
            PrimeWithTarget = null;

            if (f.InjurySystem == null)
                f.InjurySystem = f.PerformedBy.GetBestInjurySystem();

            var attackerAdvantage = f.PerformedBy.GetFinalStats()[Stat.Fight] - f.TargetIfAny.GetFinalStats()[Stat.Fight];
            
            ui.Log.Info(new LogEntry($"{f.PerformedBy} fought {f.TargetIfAny}",stack.Round,f.PerformedBy));
            
            //inflict damage on the target
            f.InjurySystem?.Apply(new SystemArgs(world,ui,
                attackerAdvantage + 20, //Even fighting someone of lower Fight results in injury so add 20

                f.PerformedBy,
                f.TargetIfAny,
                stack.Round));
            
            //inflict damage back again
            f.TargetIfAny.GetBestInjurySystem()?.Apply(new SystemArgs(world,ui,
                -attackerAdvantage + 20,
                f.TargetIfAny,
                f.PerformedBy,
                stack.Round));

            foreach (var a in new []{f.TargetIfAny, f.PerformedBy})
            {
                //fighting makes you tired
                world.AdjectiveFactory.Create(world,a,new Guid("378449b2-2b29-4849-81b6-04433dba02a9") )
                    //expires at the end of the next round
                    .WithExpiry(2);
            }
            
        }

        public override bool HasTargets(IActor performer)
        {
            return GetTargets(performer).Any();
        }

        public override IEnumerable<IHasStats> GetTargets(IActor performer)
        {
            return performer.GetCurrentLocationSiblings(false);
        }

        public override string ToString()
        {
            if(Owner is IActor a)
                return $"{Name} [{a.FightVerb}]";

            return base.ToString();
        }
    }
}
