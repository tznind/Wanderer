using Wanderer.Actors;

namespace Wanderer.Systems
{
    /// <summary>
    /// A system for applying a range of possibly cumulative effects on an
    /// <see cref="IActor"/> e.g. injuries
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Apply the system to the recipient
        /// </summary>
        void Apply(SystemArgs args);
    }
}
