namespace Wanderer
{
    /// <summary>
    /// A direction of movement in the 3D coordinate space of a Wanderer <see cref="IWorld.Map"/>
    /// </summary>
    public enum Direction
    {
        None = 0,

        /// <summary>
        /// Vertically up e.g. the vector 0,0,1
        /// </summary>
        Up,

        /// <summary>
        /// Vertically down e.g. the vector 0,0,-1
        /// </summary>
        Down,

        /// <summary>
        /// Forwards on the y axis e.g. 0,1,0
        /// </summary>
        North,

        /// <summary>
        /// Backwards on the y axis e.g. 0,-1,0
        /// </summary>
        South,

        /// <summary>
        /// Right on the x axis e.g. 1,0,0
        /// </summary>
        East,

        /// <summary>
        /// Left on the x axis e.g. -1,0,0
        /// </summary>
        West
    }
}