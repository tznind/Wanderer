using System.Linq;
using Newtonsoft.Json;
using StarshipWanderer.Actors;
using StarshipWanderer.Extensions;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
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