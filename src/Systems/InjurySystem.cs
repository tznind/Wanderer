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
            if (args.Recipient.Adjectives.OfType<Injured>().Count() > 2)
            {
                args.Recipient.Kill(args.UserInterface,args.Round);
                args.UserInterface.Log.Info($"{args.Recipient} died from their injuries",args.Round);
            }
            else
            {
                var newInjury = new Injured(args.Recipient);   
                args.Recipient.Adjectives.Add(newInjury);
                args.UserInterface.Log.Info($"{args.Recipient} gained {newInjury}",args.Round);
            }
        }

        public void Apply(SystemArgs args)
        {
            var regions = Enum.GetValues(typeof(InjuryRegion)).Cast<InjuryRegion>().ToArray();

            Apply(args,regions[args.Intensity % regions.Length]);
        }

        public IEnumerable<Injured> GetAvailableInjuries(IActor actor)
        {
            yield return new Injured(actor);
        }
    }
}