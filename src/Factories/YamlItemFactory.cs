using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Factories
{
    public class YamlItemFactory : ItemFactory
    {
        [JsonConstructor]
        private YamlItemFactory(IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
            
        }

        public YamlItemFactory(string yaml, IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
            var deserializer = new Deserializer();
            Blueprints = deserializer.Deserialize<ItemBlueprint[]>(yaml);
        }
    }
}