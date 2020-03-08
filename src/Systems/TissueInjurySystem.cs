using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Adjectives.RoomOnly;

namespace Wanderer.Systems
{

    public class TissueInjurySystem : InjurySystem
    {
        public TissueInjurySystem()
        {

            Identifier =  new Guid("9b137f26-834d-4033-ae36-74ab578f5868");

            ResistWorsen.Immune.Add(typeof(Tough));
            ResistWorsen.Vulnerable.Add(typeof(Stale));
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


    }
}