﻿using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Places;
using System.Linq;
using Wanderer.Extensions;

namespace Wanderer.Systems
{
    public class FireInjurySystem : InjurySystem
    {
        public override Guid Identifier { get;set; } = new Guid("3c0dd6d7-1f84-44ad-b446-323bd747b09b");

        public override IEnumerable<Injured> GetAvailableInjuries(IHasStats actor)
        {
            for(double i = 1 ; i <=5;i++)
                yield return new Injured(
                    GetDescription(i),actor,1,InjuryRegion.None,this){

                        //mark the injury as comming from hunger system
                        Identifier = Identifier
                    };
        }

        private string GetDescription(double severity)
        {
            if(severity <= 1.0001)
                return "Smoking";
            if(severity <= 2.0001)
                return "Smouldering";
            if(severity <= 3.0001)
                return "Burning";
            if(severity <= 4.0001)
                return "Flaming";
            if(severity <= 5.0001)
                return "Conflagration";

            return "Inferno";
        }

        public override void Heal(Injured injured, IUserinterface ui, Guid round)
        {
            injured.Severity -= 2;
            if(injured.Severity <= 0)
                injured.Owner.Adjectives.Remove(injured);
            else
                injured.Name = GetDescription(injured.Severity);
        }

        public override bool IsHealableBy(IActor actor, Injured injured, out string reason)
        {
            reason = "it's fire!";
            return false;
        }

        public override void Worsen(Injured injured, IUserinterface ui, Guid round)
        {
            
            if(injured.Owner is IPlace p)
            {
                //rooms set other rooms on fire!
                foreach(var adjacent in p.World.Map.GetAdjacentPlaces(p,false))
                    this.Apply(new SystemArgs(p.World,ui,1,null,p,round));

                //and the people in them
                foreach(var actor in p.Actors)
                    this.Apply(new SystemArgs(p.World,ui,1,null,actor,round));
            }
            
            if(injured.Owner is IActor a)
            {
                var world =a.CurrentLocation.World;
                var region =  ((InjuryRegion[])Enum.GetValues(typeof(InjuryRegion))).ToList().GetRandom(world.R);

                //This should be a soft tissue injury rebranded as a a burn
                var burn = new Injured("Burnt " + region,a,injured.Severity,region,world.InjurySystems[0]);

                a.Adjectives.Add(burn);
            }
        }

        protected override IEnumerable<InjuryRegion> GetAvailableInjuryLocations(SystemArgs args)
        {
            yield return InjuryRegion.None;
        }

        protected override bool ShouldNaturallyHealImpl(Injured injured, int roundsSeenCount)
        {
            return false;
        }

        protected override bool ShouldWorsenImpl(Injured injury, int roundsSeen)
        {
            return true;
        }

        protected override bool IsWithinNaturalHealingThreshold(Injured injured)
        {
            return false;
        }

        public override bool HasFatalInjuries(IInjured injured, out string diedOf)
        {
            diedOf = null;
            return false;
        }
    }
}