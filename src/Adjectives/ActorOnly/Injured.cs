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
        public InjuryRegion Region { get; set; }
        public int Severity { get; set; }
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
                ui.Log.Info($"{Name} became infected",round);
                Name = "Infected " + Name;
            }
            else
                ui.Log.Info($"{Name} got worse", round);

            Severity++;
        }


        public void Heal(IUserinterface ui, Guid round)
        {
            Owner.Adjectives.Remove(this);
            ui.Log.Info($"{Name} was healed",round);
        }

        public Injured(string name,IActor actor, int severity,InjuryRegion region):base(actor)
        {
            Severity = severity;
            Region = region;
            IsPrefix = false;
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
            int worsenRate = 1;

            if (OwnerActor.Has<Tough>(true))
                worsenRate--;

            if (OwnerActor.CurrentLocation.Has<Stale>())
                worsenRate++;

            return worsenRate != 0 && _roundsSeen.Count % (Severity*2 / worsenRate) == 0;
        }

        public void OnRoundEnding(IUserinterface ui, Guid round)
        {
            if(HasReachedFatalThreshold())
                OwnerActor.Kill(ui,round);
        }

        public virtual bool HasReachedFatalThreshold()
        {
            //Combined total of serious wounds (2 or higher) severity is 10
            return Owner.Adjectives.OfType<Injured>().Where(i => i.Severity > 1).Sum(i => i.Severity) >= 10;
        }

        public override IEnumerable<string> GetDescription()
        {
            yield return "Reduces Stats";
            yield return "Leads to Death";
        }
    }
}