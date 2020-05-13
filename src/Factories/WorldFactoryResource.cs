using System;

namespace Wanderer.Factories
{
    /// <summary>
    /// Describes a piece of yaml text that will be loaded by <see cref="WorldFactory"/>
    /// during world creation
    /// </summary>
    public class WorldFactoryResource
    {
        public string Location { get; set; }
        public string Content { get; set; }

        public WorldFactoryResource(string location, string content)
        {
            Location = location;
            Content = content;
        }

        private string GetPath()
        {
            var idx = Location.LastIndexOfAny(new[] {'/', '\\'});

            return Location.Substring(0, idx + 1);
        }

        /// <summary>
        /// Return true if this and the <paramref name="other"/> share the same directory
        /// or if <paramref name="other"/> is in a sub directory beneath this
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool SharesPath(WorldFactoryResource other)
        {
            return other.GetPath().StartsWith(GetPath(),StringComparison.CurrentCultureIgnoreCase);
        }
    }
}