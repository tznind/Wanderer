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


        public double NaturalHealThreshold {get;set;} = 20;

        public double NaturalHealRate {get;set;} = 10;

        
        public bool SyncDescriptions{get;set;}

        public string WorsenVerb {get;set;} = "got worse";

        public bool Infection {get;set;}

        public Spreading Spreads{get;set;}

        public virtual void Apply(SystemArgs args)
        {
            if(args.Intensity < 0 )
                return;

            if (args.Recipient == null)
                return;

            var candidate = GetBlueprintFor(args.Intensity);

            if(candidate == null)
                throw new Exception("No Injury  found for severity " + args.Intensity);

            var newInjury = new Injured(candidate.Name,args.Recipient,args.Intensity,candidate.Region,this);

            args.Recipient.Adjectives.Add(newInjury);
            args.UserInterface.Log.Info(new LogEntry($"{args.Recipient} gained {newInjury}", args.Round,args.Room.GetPoint()));
        }

        protected virtual InjuryBlueprint GetBlueprintFor(double intensity)
        {
            return Injuries
                .OrderBy(a =>
                //find the closest intensity injury to what is desired
                Math.Abs(intensity - a.Severity))
                .FirstOrDefault();
        }

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

            double ratio = 1;

            var haveTypes = injury.Owner.GetAllHaves().Select(h=>h.GetType()).Distinct();

            //If you have something that makes you immune to worsening
            if(ResistWorsen.Immune.Intersect(haveTypes).Any())
                return false;
            
            //If you have something that makes you resist worsening
            if(ResistWorsen.Resist.Intersect(haveTypes).Any())
                ratio *= 2;
            
            //If you have something that makes you vulnerable to worsening
            if(ResistWorsen.Vulnerable.Intersect(haveTypes).Any())
                ratio *= 0.5;

            return roundsSeen > ratio * WorsenRate;
        }

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

            return  roundsSeenCount > NaturalHealRate;
        }


        public virtual void Worsen(Injured injured, IUserinterface ui, Guid round)
        {
            injured.Severity+=10;

            //if injury names should be updated with severity
            if(SyncDescriptions)
            {
                var newInjury = GetBlueprintFor(injured.Severity);

                if(newInjury != null)
                    injured.Name = newInjury.Name;
            }
            
            //if wounds can become infected
            if (!injured.IsInfected && Infection)
            {
                injured.IsInfected = true;
                ui.Log.Info(new LogEntry($"{injured.Name} became infected",round,injured.Owner as IActor));
                injured.Name = "Infected " + injured.Name;
            }
            else
                ui.Log.Info(new LogEntry($"{injured.Name} {WorsenVerb}", round,injured.Owner as IActor));

            if(Spreads != null)
            {
                
                /*
            
            if(injured.Owner is IRoom p)
            {
                //rooms set other rooms on fire!
                foreach(var adjacent in p.World.Map.GetAdjacentRooms(p,false))
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
            }*/
            }
        }

        
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
            return injured.Severity <= NaturalHealThreshold;
        }
    }
}