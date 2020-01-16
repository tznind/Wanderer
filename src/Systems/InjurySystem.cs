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
                (int)a.Severity == (int)Math.Min(worst, args.Intensity / 10) && a.Region == region);

            if(newInjury == null)
                throw new Exception("No Injury  found for severity " + args.Intensity);

            args.Recipient.Adjectives.Add(newInjury);
            args.UserInterface.Log.Info(new LogEntry($"{args.Recipient} gained {newInjury}", args.Round,args.Recipient));
        }

        public void Apply(SystemArgs args)
        {
            var regions = Enum.GetValues(typeof(InjuryRegion)).Cast<InjuryRegion>().ToArray();

            //Generate a random region excluding None
            Apply(args,regions[(int) Math.Abs(args.Intensity % (regions.Length-1)+1)]);
        }

        public virtual IEnumerable<Injured> GetAvailableInjuries(IActor actor)
        {
            foreach (InjuryRegion region in Enum.GetValues(typeof(InjuryRegion)))
            {
                double severity = 0;
                foreach (var s in new string[]{"Bruised","Cut","Lacerated","Fractured","Broken","Detached"})
                    yield return new Injured(s + " " + region, actor, severity++, region,this);
            }
        }

        public bool HasFatalInjuries(IActor owner, out string diedOf)
        {
            //Combined total of serious wounds (2 or higher) severity is 10
            if (owner.Adjectives.OfType<Injured>().Where(i => i.Severity > 1).Sum(i => i.Severity) >= 10)
            {
                diedOf = "your wounds";
                return true;
            }

            diedOf = null;
            return false;
        }
    }
}