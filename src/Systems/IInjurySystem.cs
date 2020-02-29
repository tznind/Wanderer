using System;
using System.Collections.Generic;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Adjectives;

namespace Wanderer.Systems
{
    /// <inheritdoc />
    public interface IInjurySystem: ISystem
    {
        

        void Apply(SystemArgs args, InjuryRegion region);
        IEnumerable<Injured> GetAvailableInjuries(IHasStats actor);

        /// <summary>
        /// Returns true if the <paramref name="owner"/> is so injured (by this system)
        /// that they should die.
        /// </summary>
        /// <param name="injured"></param>
        /// <param name="diedOf"></param>
        /// <returns></returns>
        bool HasFatalInjuries(IInjured injured, out string diedOf);

        /// <summary>
        /// Returns true if the injury should worsen
        /// </summary>
        /// <param name="injured"></param>
        /// <param name="roundsSeen">Number of rounds since it last got worse</param>
        /// <returns></returns>
        bool ShouldWorsen(Injured injured, int roundsSeen);

        /// <summary>
        /// True if the <paramref name="actor"/> can heal the <paramref name="injured"/>
        /// adjective.  This is usually a test done by the <see cref="HealAction"/>
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="injured"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        bool IsHealableBy(IActor actor, Injured injured, out string reason);
        bool ShouldNaturallyHeal(Injured injured, int roundsSeenCount);
        
        /// <summary>
        /// Make the given <paramref name="injured"/> worse (i.e. progress to infection,
        /// generally bring you closer to dying / destruction).
        /// </summary>
        /// <param name="injured"></param>
        /// <param name="ui"></param>
        /// <param name="round"></param>
        void Worsen(Injured injured, IUserinterface ui, Guid round);

        /// <summary>
        /// Heal the given <paramref name="injured"/> condition
        /// </summary>
        /// <param name="injured"></param>
        /// <param name="ui"></param>
        /// <param name="round"></param>
        void Heal(Injured injured, IUserinterface ui, Guid round);

        /// <summary>
        /// Kill/destroy the owner of the given <paramref name="injured"/> (usually
        /// because they <see cref="HasFatalInjuries"/>)
        /// </summary>
        /// <param name="injured"></param>
        /// <param name="ui"></param>
        /// <param name="round"></param>
        /// <param name="diedOf"></param>
        void Kill(Injured injured, IUserinterface ui, Guid round, string diedOf);
    }
}