using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Systems
{
    public class HungerInjurySystem : InjurySystem
    {
        public HungerInjurySystem()
        {
            Identifier = new Guid("89c18233-5250-4445-8799-faa9a888fb7f");

            base.Injuries.Add(new InjuryBlueprint("Peckish",10));
            base.Injuries.Add(new InjuryBlueprint("Hungry",20));
            base.Injuries.Add(new InjuryBlueprint("Famished",30));
            base.Injuries.Add(new InjuryBlueprint("Ravenous",40));
            base.Injuries.Add(new InjuryBlueprint("Starved",50));

            FatalThreshold = 60;
            FatalVerb = "starvation";

            //does not get better by itself
            NaturalHealThreshold = 0;

            SyncDescriptions = true;
        }
    }
}