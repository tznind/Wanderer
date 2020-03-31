using System;
using Newtonsoft.Json;
using Wanderer.Actors;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Adjectives
{
    public class Injured : Adjective, IInjured
    {
        public IInjurySystem InjurySystem { get; set; }
        public InjuryRegion Region { get; set; }
        public double Severity { get; set; }
        public bool IsInfected { get; set; }

        
        public void Worsen(IUserinterface ui, Guid round)
        {
            InjurySystem.Worsen(this, ui, round);
        }

        public void Heal(IUserinterface ui, Guid round)
        {
            InjurySystem.Heal(this, ui, round);
        }

        [JsonConstructor]
        protected Injured()
        {

        }

        public Injured(string name,IHasStats owner, double severity,InjuryRegion region,IInjurySystem system):base(owner)
        {
            Severity = severity;
            Region = region;
            IsPrefix = false;
            InjurySystem = system;

            BaseStats[Stat.Fight] = -0.5 * severity;
            Name = name;

            BaseBehaviours.Add(new InjuredBehaviour(this));
        }
        
        
        public bool IsHealableBy(IActor actor, out string reason)
        {
            return InjurySystem.IsHealableBy(actor, this, out reason);
        }
    }
}