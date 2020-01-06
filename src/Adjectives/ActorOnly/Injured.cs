using System;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Stats;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Adjectives.ActorOnly
{
    public class Injured : Adjective, IBehaviour
    {
        public InjuryRegion Region { get; set; }
        public int Severity { get; set; }

        public Injured(string name,IActor actor, int severity,InjuryRegion region):base(actor)
        {
            Severity = severity;
            Region = region;
            IsPrefix = false;
            BaseStats[Stat.Fight] = -5 * severity;
            Name = name;

            BaseBehaviours.Add(this);
        }

        public void OnPush(IUserinterface ui, ActionStack stack, Frame frame)
        {
        }

        public void OnRoundEnding(IUserinterface ui, Guid round)
        {
            if(HasReachedFatalThreshold())
                ((IActor)Owner).Kill(ui,round);
        }

        /// <summary>
        /// Return true if the <see cref="Adjective.Owner"/> has a passed a critical threshold
        /// of injuries and should die from the wounds.
        /// </summary>
        /// <returns></returns>
        protected bool HasReachedFatalThreshold()
        {
            //Combined total of serious wounds (2 or higher) severity is 10
            return Owner.Adjectives.OfType<Injured>().Where(i => i.Severity > 1).Sum(i => i.Severity) >= 10;
        }
    }
}