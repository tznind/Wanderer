using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Adjectives.RoomOnly;
using Wanderer.Factories.Blueprints;
using Wanderer.Stats;

namespace Wanderer.Systems
{
    public class InjurySystem : IInjurySystem
    {
        public Guid Identifier { get; set; }

        /// <summary>
        /// Describes how the injuries inflicted by this system are healed e.g.
        /// "healed", "put out", "solved by eating"
        /// </summary>
        public string HealVerb { get; set; } = "healed";

        /// <summary>
        /// If set then actors with this stat can attempt to heal
        /// </summary>
        public Stat? HealerStat { get; set; }

        /// <summary>
        /// How much <see cref="HealerStat"/> is required for each point of Injury Severity
        /// </summary>
        public double HealerStatMultiplier { get; set; } = 1.0;

        /// <summary>
        /// The number of rounds it takes for a wound to get worse
        /// </summary>
        public int WorsenRate { get; set; } = 10;


        /// <summary>
        /// Types of <see cref="IAdjective"/> which make you resistant to this type of damage
        /// </summary>
        public Resistances ResistInflict { get; set; } = new Resistances();

        /// <summary>
        /// Types of <see cref="IAdjective"/> which make you resistant to this type of damage
        /// getting worse.
        /// </summary>
        public Resistances ResistWorsen { get; set; } = new Resistances();

        public List<InjuryBlueprint> Injuries { get; set; } = new List<InjuryBlueprint>();


        public virtual void Apply(SystemArgs args)
        {
            if(args.Intensity < 0 )
                return;

            if (args.Recipient == null)
                return;

            var candidates = 
            Injuries
                .OrderBy(a =>
                //find the closest intensity injury to what is desired
                Math.Abs(args.Intensity - a.Severity))
                .FirstOrDefault();

            if(candidates == null)
                throw new Exception("No Injury  found for severity " + args.Intensity);

            newInjury.Severity = args.Intensity;

            args.Recipient.Adjectives.Add(newInjury);
            args.UserInterface.Log.Info(new LogEntry($"{args.Recipient} gained {newInjury}", args.Round,args.Room.GetPoint()));
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

            double worsenRate = 1;
            
            var a = injury.Owner as IActor;

            if (a != null && a.Has<Tough>(true))
                worsenRate--;

            if (a != null && a.CurrentLocation.Has<Stale>())
                worsenRate++;

            return Math.Abs(worsenRate) > 0.0001 && Math.Abs(roundsSeen % (injury.Severity*0.2 / worsenRate)) < 0.0001;



            return ShouldWorsenImpl(injury, roundsSeen);
        }

        /// <summary>
        /// Override to indicate whether the <paramref name="injury"/> should get worse.
        /// </summary>
        /// <param name="injury"></param>
        /// <param name="roundsSeen">Time since it last got worse</param>
        /// <returns></returns>
        protected abstract bool ShouldWorsenImpl(Injured injury, int roundsSeen);

        public virtual bool IsHealableBy(IActor actor, Injured injured, out string reason)
        {
            if (!HealerStat.HasValue)
            {
                reason = "cannot be " + HealVerb;
                return false;
            }
                
            var requiredStat = injured.Severity * HealerStatMultiplier;

            //harder to heal giant things
            if(injured.Owner is IActor a)
                if (a.Has<Giant>(false))
                    requiredStat *= 1.5;

            if (actor.GetFinalStats()[HealerStat.Value] > requiredStat)
            {
                reason = null;
                return true;
            }

            reason = $"{HealerStat} was too low (required {requiredStat})";
            return false;
        }

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
        
        public virtual void Heal(Injured injured, IUserinterface ui, Guid round)
        {
            injured.Owner.Adjectives.Remove(injured);
            ui.Log.Info(new LogEntry($"{injured.Name} was {HealVerb}",round,injured.Owner as IActor));
        }

        

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