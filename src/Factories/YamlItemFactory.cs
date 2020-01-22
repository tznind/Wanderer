using YamlDotNet.Serialization;

namespace StarshipWanderer.Factories
{
    public class YamlItemFactory : ItemFactory
    {
        public YamlItemFactory(string yaml, IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
            var deserializer = new Deserializer();
            Blueprints = deserializer.Deserialize<ItemBlueprint[]>(yaml);
        }
    }
}