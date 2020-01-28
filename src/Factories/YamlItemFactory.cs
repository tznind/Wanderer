using Newtonsoft.Json;
using StarshipWanderer.Conditions;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Factories.Blueprints;
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
            var deserializer = 
                new DeserializerBuilder()
                    .WithTypeConverter(new ConditionYamlTypeConverter())
                    .Build();

            Blueprints = deserializer.Deserialize<ItemBlueprint[]>(yaml);
        }
    }
}