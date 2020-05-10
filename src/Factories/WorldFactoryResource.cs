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

    }
}