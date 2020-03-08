using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Adjectives.RoomOnly;
using Wanderer.Factories.Blueprints;
using Wanderer.Stats;

namespace Wanderer.Systems
{

    public class TissueInjurySystem : InjurySystem
    {
        public TissueInjurySystem()
        {
            Identifier =  new Guid("9b137f26-834d-4033-ae36-74ab578f5868");


            foreach (InjuryRegion region in Enum.GetValues(typeof(InjuryRegion)))
            {
                if(region == InjuryRegion.None)
                    continue;

                double severity = 0;
                foreach (var s in new string[]{"Bruised","Cut","Lacerated","Fractured","Broken","Detached"})
                    Injuries.Add(new InjuryBlueprint(s + " " + region, (severity+=10)+(int)region -1, region));
            }

            ResistWorsen.Immune.Add(typeof(Tough));
            ResistWorsen.Vulnerable.Add(typeof(Stale));

            HealerStat = Stat.Savvy;
            HealerStatMultiplier = 0.5;

            NaturalHealRate = 10;
            NaturalHealThreshold = 20;
        }

    }
}