using Newtonsoft.Json;
using StarshipWanderer.Actions;
using StarshipWanderer.Compilation;
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
            Blueprints = Compiler.Instance.Deserializer.Deserialize<ItemBlueprint[]>(yaml);
        }
    }
}