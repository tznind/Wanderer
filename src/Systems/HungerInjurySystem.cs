using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives;

namespace Wanderer.Systems
{
    public class HungerInjurySystem : InjurySystem
    {
        public HungerInjurySystem()
        {
            Identifier = new Guid("89c18233-5250-4445-8799-faa9a888fb7f");
        }

        public override IEnumerable<Injured> GetAvailableInjuries(IHasStats actor)
        {
            for(double i = 10 ; i <=50;i+=10)
                yield return new Injured(
                    GetDescription(i),actor,1,InjuryRegion.None,this){

                        //mark the injury as comming from hunger system
                        Identifier = Identifier
                    };
        }

        private string GetDescription(double severity)
        {
            if(severity <= 10.0001)
                return "Peckish";
            if(severity <= 20.0001)
                return "Hungry";
            if(severity <= 30.0001)
                return "Famished";
            if(severity <= 40.0001)
                return "Ravenous";

            return "Starved";
        }

        public override void Heal(Injured injured, IUserinterface ui, Guid round)
        {
            injured.Severity -= 20;
            if(injured.Severity <= 0)
                injured.Owner.Adjectives.Remove(injured);
            else
                injured.Name = GetDescription(injured.Severity);
        }


        public override void Worsen(Injured injured, IUserinterface ui, Guid round)
        {
            injured.Severity+=10;
            injured.Name = GetDescription(injured.Severity);
        }

        public override bool HasFatalInjuries(IInjured injured, out string diedOf)
        {
            diedOf = "Hunger";
            return injured.Severity>=70;
        }


        protected override bool ShouldNaturallyHealImpl(Injured injured, int roundsSeenCount)
        {
            return false;
        }

        protected override bool IsWithinNaturalHealingThreshold(Injured injured)
        {
            return false;
        }

        protected override bool ShouldWorsenImpl(Injured injury, int roundsSeen)
        {
            return roundsSeen > injury.Severity * 0.2;
        }
    }
}