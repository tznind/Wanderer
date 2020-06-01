using Wanderer.Systems;

namespace Wanderer.Compilation
{
    /// <summary>
    /// Describes an operation to perform in the world typically as a result of an action, dialogue choice etc
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// Applies the effect to one or more of the things described in <paramref name="args"/> e.g. <see cref="SystemArgs.Room"/>
        /// </summary>
        /// <param name="args"></param>
        void Apply(SystemArgs args);
    }
}