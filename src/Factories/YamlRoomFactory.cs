using Newtonsoft.Json;
using StarshipWanderer.Factories.Blueprints;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Factories
{
    public class YamlRoomFactory : RoomFactory
    {
        [JsonConstructor]
        private YamlRoomFactory(IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {

        }

        public YamlRoomFactory(string yaml, IAdjectiveFactory adjectiveFactory) : base(adjectiveFactory)
        {
            var deserializer = new Deserializer();
            Blueprints = deserializer.Deserialize<RoomBlueprint[]>(yaml);
        }
    }
}