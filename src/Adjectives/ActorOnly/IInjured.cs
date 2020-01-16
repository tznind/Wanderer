using System;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Adjectives.ActorOnly
{
    public interface IInjured : IAdjective
    {
        IActor OwnerActor { get; set; }

        InjuryRegion Region { get; set; }
        double Severity { get; set; }
        
        bool IsInfected { get; set; }

        /// <summary>
        /// Make the current wound worse
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="round"></param>
        void Worsen(IUserinterface ui, Guid round);

        /// <summary>
        /// Heals the current wound
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="round"></param>
        void Heal(IUserinterface ui,  Guid round);
    }
}