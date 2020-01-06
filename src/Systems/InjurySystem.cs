using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives.ActorOnly;

namespace StarshipWanderer.Systems
{
    public class InjurySystem : IInjurySystem
    {
        public void Apply(SystemArgs args, InjuryRegion region)
        {
            if(args.Intensity < 0 || region == InjuryRegion.None)
                return;
            
            var available = GetAvailableInjuries(args.Recipient).ToArray();

            var worst = available.Max(i => i.Severity);

            var newInjury = available.FirstOrDefault(a =>
                a.Severity == Math.Min(worst, args.Intensity / 10) && a.Region == region);

            args.Recipient.Adjectives.Add(newInjury);
            args.UserInterface.Log.Info($"{args.Recipient} gained {newInjury}", args.Round);
        }

        public void Apply(SystemArgs args)
        {
            var regions = Enum.GetValues(typeof(InjuryRegion)).Cast<InjuryRegion>().ToArray();

            //Generate a random region excluding None
            Apply(args,regions[(args.Intensity % (regions.Length-1)+1)]);
        }

        public virtual IEnumerable<Injured> GetAvailableInjuries(IActor actor)
        {
            foreach (InjuryRegion region in Enum.GetValues(typeof(InjuryRegion)))
            {
                int severity = 0;
                foreach (var s in new string[]{"Bruised","Cut","Lacerated","Fractured","Broken","Detached"})
                    yield return new Injured(s + " " + region, actor, severity++, region);
            }
        }
    }
}