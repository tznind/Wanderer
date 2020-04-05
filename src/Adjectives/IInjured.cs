using System;
using Wanderer.Actors;
using Wanderer.Systems;

namespace Wanderer.Adjectives
{
    public interface IInjured : IAdjective
    {
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

        /// <summary>
        /// Return true if the wound is healable by the skills of the given
        /// <paramref name="actor"/>
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        bool IsHealableBy(IActor actor, out string reason);
    }
}