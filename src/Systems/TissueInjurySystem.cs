using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Adjectives.ActorOnly;
using Wanderer.Adjectives.RoomOnly;
using Wanderer.Extensions;
using Wanderer.Stats;

namespace Wanderer.Systems
{
    public class TissueInjurySystem : InjurySystem
    {
        public override Guid Identifier { get; set; } = new Guid("9b137f26-834d-4033-ae36-74ab578f5868");

        protected override IEnumerable<InjuryRegion> GetAvailableInjuryLocations(SystemArgs args)
        {
            return Enum.GetValues(typeof(InjuryRegion)).Cast<InjuryRegion>().Where(r=>r != InjuryRegion.None);
        }

        public override IEnumerable<Injured> GetAvailableInjuries(IActor actor)
        {
            foreach (InjuryRegion region in Enum.GetValues(typeof(InjuryRegion)))
            {
                double severity = 0;
                foreach (var s in new string[]{"Bruised","Cut","Lacerated","Fractured","Broken","Detached"})
                    yield return new Injured(s + " " + region, actor, severity++, region,this);
            }
        }

        protected override bool ShouldWorsenImpl(Injured injury, int roundsSeen)
        {
            double worsenRate = 1;
            
            var a = injury.Owner as IActor;

            if (a != null && a.Has<Tough>(true))
                worsenRate--;

            if (a != null && a.CurrentLocation.Has<Stale>())
                worsenRate++;

            return Math.Abs(worsenRate) > 0.0001 && Math.Abs(roundsSeen % (injury.Severity*2 / worsenRate)) < 0.0001;
        
        }

        public override bool IsHealableBy(IActor actor, Injured injured, out string reason)
        {
            var requiredSavvy = injured.Severity * 5;

            //harder to heal giant things
            if(injured.Owner is IActor a)
                if (a.Has<Giant>(false))
                    requiredSavvy *= 1.5;

            if (actor.GetFinalStats()[Stat.Savvy] > requiredSavvy)
            {
                reason = null;
                return true;
            }

            reason = $"Savvy was too low (required {requiredSavvy})";
            return false;
        }

        protected override bool ShouldNaturallyHealImpl(Injured injured, int roundsSeen)
        {
            return roundsSeen >= 10;
        }

        public override void Worsen(Injured injured, IUserinterface ui, Guid round)
        {
            if (!injured.IsInfected)
            {
                injured.IsInfected = true;
                ui.Log.Info(new LogEntry($"{injured.Name} became infected",round,injured.Owner as IActor));
                injured.Name = "Infected " + injured.Name;
            }
            else
                ui.Log.Info(new LogEntry($"{injured.Name} got worse", round,injured.Owner as IActor));

            injured.Severity++;
        }

        public override void Heal(Injured injured, IUserinterface ui, Guid round)
        {
            injured.Owner.Adjectives.Remove(injured);
            ui.Log.Info(new LogEntry($"{injured.Name} was healed",round,injured.Owner as IActor));
        }
    }
}