namespace Wanderer.Factories
{
    /// <summary>
    /// Describes a piece of yaml text that will be loaded by <see cref="WorldFactory"/>
    /// during world creation
    /// </summary>
    public abstract class WorldFactoryResource
    {
        public string Location { get; set; }
        public string Content { get; set; }

        protected WorldFactoryResource(string location, string content)
        {
            Location = location;
            Content = content;
        }

        /// <summary>
        /// Return true if this and the <paramref name="other"/> share the same directory
        /// or if <paramref name="other"/> is in a sub directory beneath this
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool SharesPath(WorldFactoryResource other);
    }
}