using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;

namespace Wanderer.Systems
{

    public class TissueInjurySystem : InjurySystem
    {
        public TissueInjurySystem()
        {
            Identifier =  new Guid("9b137f26-834d-4033-ae36-74ab578f5868");
        }

        public override IEnumerable<Injured> GetAvailableInjuries(IHasStats actor)
        {
            foreach (InjuryRegion region in Enum.GetValues(typeof(InjuryRegion)))
            {
                double severity = 0;
                foreach (var s in new string[]{"Bruised","Cut","Lacerated","Fractured","Broken","Detached"})
                    yield return new Injured(s + " " + region, actor, severity+=10, region,this);
            }
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

            injured.Severity+=10;
        }

    }
}