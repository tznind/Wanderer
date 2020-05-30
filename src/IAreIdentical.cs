namespace Wanderer
{
    /// <summary>
    /// Interface for comparing two separate game objects to see if the user would consider them the same (e.g. 2 gold coins).  Helps with merging stacks, deduplication etc
    /// </summary>
    public interface IAreIdentical
    {
        /// <summary>
        /// Returns true if the user would consider 2 objects the same e.g. 2 "Rusty Broken Gun"
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool AreIdentical(object other);
    }
}