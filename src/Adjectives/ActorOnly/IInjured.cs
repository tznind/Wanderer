﻿using System;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Adjectives.ActorOnly
{
    public interface IInjured : IAdjective
    {
        InjuryRegion Region { get; set; }
        int Severity { get; set; }
        
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

        
        /// <summary>
        /// Return true if the <see cref="IAdjective.Owner"/> has a passed a critical threshold
        /// of injuries and should die from the wounds.
        /// </summary>
        /// <returns></returns>
        bool HasReachedFatalThreshold();
    }
}