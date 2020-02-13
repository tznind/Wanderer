using System;
using System.Collections.Generic;
using Wanderer.Actors;
using Wanderer.Adjectives.ActorOnly;

namespace Wanderer.Systems
{
    /// <inheritdoc />
    public interface IInjurySystem: ISystem
    {
        

        void Apply(SystemArgs args, InjuryRegion region);
        IEnumerable<Injured> GetAvailableInjuries(IActor actor);

        /// <summary>
        /// Returns true if the <paramref name="owner"/> is so injured (by this system)
        /// that they should die.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="diedOf"></param>
        /// <returns></returns>
        bool HasFatalInjuries(IActor owner, out string diedOf);

        /// <summary>
        /// Returns true if the injury should worsen
        /// </summary>
        /// <param name="injured"></param>
        /// <param name="roundsSeen">Number of rounds since it last got worse</param>
        /// <returns></returns>
        bool ShouldWorsen(Injured injured, int roundsSeen);

        bool IsHealableBy(IActor actor, Injured injured, out string reason);
        bool ShouldNaturallyHeal(Injured injured, in int roundsSeenCount);
        
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
    }
}