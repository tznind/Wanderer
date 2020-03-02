using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives;

namespace Wanderer.Systems
{
    public abstract class InjurySystem : IInjurySystem
    {
        public abstract Guid Identifier { get; set; }
        
        public virtual void Apply(SystemArgs args)
        {
            var regions = GetAvailableInjuryLocations(args).ToArray();
            
            //Generate a random region
            Apply(args,regions[(int) Math.Abs(args.Intensity % regions.Length)]);
        }

        protected abstract IEnumerable<InjuryRegion> GetAvailableInjuryLocations(SystemArgs args);


        public virtual void Apply(SystemArgs args, InjuryRegion region)
        {
            if(args.Intensity < 0 )
                return;

            if (args.Recipient == null)
                return;

            var newInjury = 
            GetAvailableInjuries(args.Recipient)
                .OrderBy(a =>
                //find the closest intensity injury to what is desired
                Math.Abs(args.Intensity - a.Severity)).Where(a=> a.Region == region)
                .FirstOrDefault();

            if(newInjury == null)
                throw new Exception("No Injury  found for severity " + args.Intensity + " Region " + region);

            newInjury.Severity = args.Intensity;

            args.Recipient.Adjectives.Add(newInjury);
            args.UserInterface.Log.Info(new LogEntry($"{args.Recipient} gained {newInjury}", args.Round,args.Place.GetPoint()));
        }
        public abstract IEnumerable<Injured> GetAvailableInjuries(IHasStats actor);

        public virtual bool HasFatalInjuries(IInjured injured, out string diedOf)
        {
            //Combined total of serious wounds (2 or higher) severity is 100
            if (injured.Owner.Adjectives.OfType<Injured>().Where(i => i.Severity > 1).Sum(i => i.Severity) >= 100)
            {
                diedOf = "injuries";
                return true;
            }

            diedOf = null;
            return false;
        }

        public bool ShouldWorsen(Injured injury, int roundsSeen)
        {
            if (IsWithinNaturalHealingThreshold(injury))
                return false;

            if (injury.Owner is IActor a && a.Dead)
                return false;

            return ShouldWorsenImpl(injury, roundsSeen);
        }

        /// <summary>
        /// Override to indicate whether the <paramref name="injury"/> should get worse.
        /// </summary>
        /// <param name="injury"></param>
        /// <param name="roundsSeen">Time since it last got worse</param>
        /// <returns></returns>
        protected abstract bool ShouldWorsenImpl(Injured injury, int roundsSeen);

        public abstract bool IsHealableBy(IActor actor, Injured injured, out string reason);

        public bool ShouldNaturallyHeal(Injured injured, int roundsSeenCount)
        {
            //if your dead you are not getting better
            if (injured.Owner is IActor a && a.Dead)
                return false;

            //if the wound is too bad to heal by itself
            if (!IsWithinNaturalHealingThreshold(injured))
                return false;

            return ShouldNaturallyHealImpl(injured, roundsSeenCount);
        }

        /// <summary>
        /// Return true if the <paramref name="injured"/> should have healed by now
        /// based on it's age (<paramref name="roundsSeenCount"/>)
        /// </summary>
        /// <param name="injured"></param>
        /// <param name="roundsSeenCount"></param>
        /// <returns></returns>
        protected abstract bool ShouldNaturallyHealImpl(Injured injured, int roundsSeenCount);

        public abstract void Worsen(Injured injured, IUserinterface ui, Guid round);
        public abstract void Heal(Injured injured, IUserinterface ui, Guid round);
        
        public virtual void Kill(Injured injured, IUserinterface ui, Guid round, string diedOf)
        {
            if(injured.Owner is IActor a)
                a.Kill(ui,round, diedOf);
        }


        /// <summary>
        /// Injury should get better by itself (and not worsen), override to create injury
        /// systems that do not heal by themselves or where the threshold is higher
        /// </summary>
        /// <param name="injured"></param>
        /// <returns></returns>
        protected virtual bool IsWithinNaturalHealingThreshold(Injured injured)
        {
            return injured.Severity <= 10;
        }
    }
}