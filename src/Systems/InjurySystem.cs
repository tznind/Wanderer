using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives;
using Wanderer.Factories.Blueprints;
using Wanderer.Stats;

namespace Wanderer.Systems
{
    public class InjurySystem : IInjurySystem
    {
        private const double Tolerance = 0.0001;

        /// <summary>
        /// Unique identifier for this injury system.  By default <see cref="IInjured"/> adjectives created by the system will also have this Guid
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Human readable name for this injury system e.g. Flame, Hunger etc
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// True if the injury system should be the default if none is defined (e.g. when not armed with a specific weapon - with it's own injury system).
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Describes how the injuries inflicted by this system are healed e.g. "healed", "put out", "solved by eating"
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
        /// The number of rounds it takes for a wound to get worse.  0 for injuries that never get worse once inflicted.
        /// </summary>
        public int WorsenRate { get; set; } = 10;


        /// <summary>
        /// Types of <see cref="IAdjective"/> which make you resistant to this type of damage
        /// </summary>
        public Resistances ResistInflict { get; set; } = new Resistances();

        /// <summary>
        /// Types of <see cref="IAdjective"/> which make you resistant to this type of damage getting worse.
        /// </summary>
        public Resistances ResistWorsen { get; set; } = new Resistances();

        /// <summary>
        /// Types of <see cref="IAdjective"/> prevent (immune) or ease/complicate healing injuries inflicted by this system
        /// </summary>
        public Resistances ResistHeal { get; set; } = new Resistances();

        /// <summary>
        /// Blueprints for all injuries that can be caused by this system
        /// </summary>
        public List<InjuryBlueprint> Injuries { get; set; } = new List<InjuryBlueprint>();


        /// <summary>
        /// How bad an injury can be before it will no longer heal by itself.  Set to 0 to make wounds that never heal (by themselves) 
        /// </summary>
        public double NaturalHealThreshold {get;set;} = 20;

        /// <summary>
        /// The number of rounds that must pass before a wound below the <see cref="NaturalHealThreshold"/> heals itself
        /// </summary>
        public double NaturalHealRate {get;set;} = 10;

        /// <summary>
        /// If true then injuries change name as they get better/worse e.g. "smoking" becomes "burning".  False to stick with original wording e.g. don't graduate "bruised shin" into "broken elbow"
        /// </summary>
        public bool SyncDescriptions{get;set;}

        /// <summary>
        /// How to describe the injury getting worse
        /// </summary>
        public string WorsenVerb {get;set;} = "got worse";

        /// <summary>
        /// True if injuries should become <see cref="Injured.IsInfected"/>
        /// </summary>
        public bool Infection {get;set;}

        /// <summary>
        /// Controls how/if the injuries can spread to other actors/rooms e.g. fire, plague etc.
        /// </summary>
        public Spreading Spreads{get;set;}

        /// <summary>
        /// Combined total number of injuries created by this system that should result in death
        /// </summary>
        public double FatalThreshold {get;set;} = 100;

        /// <summary>
        /// How to describe death from reaching the <see cref="FatalThreshold"/>
        /// </summary>
        public string FatalVerb { get; set; } = "injuries";


        /// <summary>
        /// Should separate applications of the injury be merged e.g. if your on fire and you get a bit hotter then it makes sense just to beef up the original instance
        /// </summary>
        public bool MergeInstances {get;set;}

        public virtual void Apply(SystemArgs args)
        {
            if(args.Intensity < 0 )
                return;

            if (args.Recipient == null)
                return;

            if(MergeInstances)
            {
                var existing = args.Recipient.Adjectives.OfType<IInjured>().Where(i=>i.InjurySystem.Equals(this)).FirstOrDefault();
                
                if(existing != null)
                {
                    Amplify(existing,args.Intensity,args.UserInterface,args.Round);
                    return;
                }
            }

            var candidate = GetBlueprintFor(args.Intensity);

            if(candidate == null)
                throw new Exception("No Injury  found for severity " + args.Intensity);

            var newInjury = new Injured(candidate.Name,args.Recipient,args.Intensity,candidate.Region,this)
            {
                Identifier = this.Identifier
            };

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
            if (injured.Owner.Adjectives.OfType<Injured>().Where(i => i.Severity > 1).Sum(i => i.Severity) >= FatalThreshold)
            {
                diedOf = FatalVerb;
                return true;
            }

            diedOf = null;
            return false;
        }

        public bool ShouldWorsen(IInjured injury, int roundsSeen)
        {
            if (WorsenRate <= 0)
                return false;

            if (IsWithinNaturalHealingThreshold(injury))
                return false;

            if (injury.Owner is IActor a && a.Dead)
                return false;

            double ratio = ResistWorsen.Calculate(injury.Owner);

            //immune
            if (Math.Abs(ratio) < Tolerance)
                return false;

            return roundsSeen > (1/ratio) * WorsenRate;
        }

        public virtual bool IsHealableBy(IActor actor, IInjured injured, out string reason)
        {
            if (!HealerStat.HasValue)
            {
                reason = "cannot be " + HealVerb;
                return false;
            }
                
            var requiredStat = injured.Severity * HealerStatMultiplier;


            var result = 
                injured.Owner is IActor i ? 
                ResistHeal.Calculate(i,false):
                ResistHeal.Calculate(injured.Owner);
            
            //result == 0
            if (Math.Abs(result) < Tolerance)
            {
                reason = $"{injured.Owner} is immune to healing (this injury type)";
                return false;
            }

            //vulnerable to healing makes you easier to heal.  ResistInflict healing makes you harder to heal
            requiredStat *= 1/result;
            
            if (actor.GetFinalStats()[HealerStat.Value] > requiredStat)
            {
                reason = null;
                return true;
            }

            reason = $"{HealerStat} was too low (required {requiredStat})";
            return false;
        }

        public bool ShouldNaturallyHeal(IInjured injured, int roundsSeenCount)
        {
            //if your dead you are not getting better
            if (injured.Owner is IActor a && a.Dead)
                return false;

            //if the wound is too bad to heal by itself
            if (!IsWithinNaturalHealingThreshold(injured))
                return false;

            return  roundsSeenCount > NaturalHealRate;
        }


        public virtual void Worsen(IInjured injured, IUserinterface ui, Guid round)
        {
            Amplify(injured,10,ui,round);

            //if wounds can become infected
            if (!injured.IsInfected && Infection)
            {
                injured.IsInfected = true;
                ui.Log.Info(new LogEntry($"{injured.Owner} {injured.Name} became infected",round,injured.Owner as IActor));
                injured.Name = "Infected " + injured.Name;
            }
            else
                ui.Log.Info(new LogEntry($"{injured.Owner} {injured.Name} {WorsenVerb}", round,injured.Owner as IActor));

            Spreads?.HandleSpreading(injured,this,ui,round);
        }

        private void Amplify(IInjured injured, double value, IUserinterface ui, Guid round)
        {
            injured.Severity+=10;

            //if injury names should be updated with severity
            if(SyncDescriptions)
            {
                var newInjury = GetBlueprintFor(injured.Severity);

                if(newInjury != null && injured.Name != newInjury.Name)
                {
                    ui.Log.Info(new LogEntry($"{injured.Owner} {injured.Name} became a {newInjury.Name}",round,injured.Owner as IActor));
                    injured.Name = newInjury.Name;
                }
            }
        }

        public virtual void Heal(IInjured injured, IUserinterface ui, Guid round)
        {
            injured.Owner.Adjectives.Remove(injured);
            ui.Log.Info(new LogEntry($"{injured.Name} was {HealVerb}",round,injured.Owner as IActor));
        }

        

        public virtual void Kill(IInjured injured, IUserinterface ui, Guid round, string diedOf)
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
        protected virtual bool IsWithinNaturalHealingThreshold(IInjured injured)
        {
            return injured.Severity <= NaturalHealThreshold;
        }
    }
}