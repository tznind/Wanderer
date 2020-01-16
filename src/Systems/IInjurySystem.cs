using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives.ActorOnly;

namespace StarshipWanderer.Systems
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
    }
}