using Newtonsoft.Json;
using StarshipWanderer.Compilation;
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
            Blueprints = Compiler.Instance.Deserializer.Deserialize<RoomBlueprint[]>(yaml);
        }
    }
}