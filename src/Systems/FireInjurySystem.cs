using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Rooms;
using System.Linq;
using Wanderer.Extensions;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Systems
{
    public class FireInjurySystem : InjurySystem
    {
        public FireInjurySystem()
        {
            Identifier = new Guid("3c0dd6d7-1f84-44ad-b446-323bd747b09b");

            base.Injuries.Add(new InjuryBlueprint("Smoking",10));
            base.Injuries.Add(new InjuryBlueprint("Smouldering",20));
            base.Injuries.Add(new InjuryBlueprint("Burning",30));
            base.Injuries.Add(new InjuryBlueprint("Flaming",40));
            base.Injuries.Add(new InjuryBlueprint("Conflagration",50));
            base.Injuries.Add(new InjuryBlueprint("Inferno",60));

            NaturalHealThreshold = 0;
            WorsenRate = 1;
            SyncDescriptions = true;
        }
    }
}