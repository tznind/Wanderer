using Newtonsoft.Json;
using StarshipWanderer.Actors;
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

        public YamlActorFactory(string yaml, string yamlDefaultSlots, IItemFactory itemFactory,
            IAdjectiveFactory adjectiveFactory) : base(itemFactory, adjectiveFactory)
        {
            var deserializer = new Deserializer();
            Blueprints = deserializer.Deserialize<ActorBlueprint[]>(yaml);

            if(!string.IsNullOrWhiteSpace(yamlDefaultSlots))
                DefaultSlots = deserializer.Deserialize<SlotCollection>(yamlDefaultSlots);
        }

    }
}