using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives.RoomOnly;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Adjectives.ActorOnly
{
    public class Injured : Adjective, IBehaviour, IInjured
    {
        public IInjurySystem InjurySystem { get; set; }
        public InjuryRegion Region { get; set; }
        public double Severity { get; set; }
        public bool IsInfected { get; set; }

        /// <summary>
        /// Tracks how many rounds have gone by
        /// </summary>
        readonly HashSet<Guid> _roundsSeen = new HashSet<Guid>();

        public IActor OwnerActor { get; set; }

        public void Worsen(IUserinterface ui,  Guid round)
        {
            if (!IsInfected)
            {
                IsInfected = true;
                ui.Log.Info(new LogEntry($"{Name} became infected",round,OwnerActor));
                Name = "Infected " + Name;
            }
            else
                ui.Log.Info(new LogEntry($"{Name} got worse", round,OwnerActor));

            Severity++;
        }


        public void Heal(IUserinterface ui, Guid round)
        {
            Owner.Adjectives.Remove(this);
            ui.Log.Info(new LogEntry($"{Name} was healed",round,OwnerActor));
        }

        public Injured(string name,IActor actor, double severity,InjuryRegion region,IInjurySystem system):base(actor)
        {
            Severity = severity;
            Region = region;
            IsPrefix = false;
            InjurySystem = system;

            BaseStats[Stat.Fight] = -5 * severity;
            Name = name;
            OwnerActor = actor;

            BaseBehaviours.Add(this);
        }

        


        public void OnPush(IUserinterface ui, ActionStack stack, Frame frame)
        {
            _roundsSeen.Add(stack.Round);

            if (Severity <= 1)
            {
                //light wounds
                if( _roundsSeen.Count >= 10)
                    Heal(ui,stack.Round);
            }
            else
            {
                //heavy wounds
                if(ShouldWorsen())
                    Worsen(ui,stack.Round);
            }
        }

        private bool ShouldWorsen()
        {
            double worsenRate = 1;

            if (OwnerActor.Has<Tough>(true))
                worsenRate--;

            if (OwnerActor.CurrentLocation.Has<Stale>())
                worsenRate++;

            return Math.Abs(worsenRate) > 0.0001 && Math.Abs(_roundsSeen.Count % (Severity*2 / worsenRate)) < 0.0001;
        }

        public void OnRoundEnding(IUserinterface ui, Guid round)
        {
            if(InjurySystem.HasFatalInjuries((IActor) Owner,out string reason))
                OwnerActor.Kill(ui,round, reason);
        }
        
        public override IEnumerable<string> GetDescription()
        {
            yield return "Reduces Stats";
            yield return "Leads to Death";
        }
    }
}