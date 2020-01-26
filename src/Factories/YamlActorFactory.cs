using Newtonsoft.Json;
using StarshipWanderer.Factories.Blueprints;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Factories
{
    public class YamlActorFactory : ActorFactory
    {

        [JsonConstructor]
        private YamlActorFactory(IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory):base(itemFactory,adjectiveFactory)
        {

        }

        public YamlActorFactory(string yaml,IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory) : base(itemFactory, adjectiveFactory)
        {
            var deserializer = new Deserializer();
            Blueprints = deserializer.Deserialize<ActorBlueprint[]>(yaml);
        }

    }
}