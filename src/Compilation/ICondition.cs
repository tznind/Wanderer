using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public interface ICondition
    {
        /// <summary>
        /// Returns true if the condition is met for the current state of the world.
        /// </summary>
        /// <param name="world">World state</param>
        /// <param name="o">Local state object for consideration e.g. current dialogue <see cref="SystemArgs"/></param>
        /// <returns></returns>
        bool IsMet(IWorld world,SystemArgs o);
    }
}