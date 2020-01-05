using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives.ActorOnly;

namespace StarshipWanderer.Systems
{
    /// <inheritdoc />
    internal interface IInjurySystem: ISystem
    {
        void Apply(SystemArgs args, InjuryRegion region);
        IEnumerable<Injured> GetAvailableInjuries(IActor actor);
    }

    public enum InjuryRegion
    {
        None,
        Head,
        Torso,
        Arm,
        Leg
    }
}