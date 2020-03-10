using System.Collections.Generic;
using Newtonsoft.Json;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;
using YamlDotNet.Serialization;

namespace Wanderer.Factories
{
    public class YamlRoomFactory : RoomFactory
    {
        [JsonConstructor]
        private YamlRoomFactory(IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {

        }

        public YamlRoomFactory(string yaml, IAdjectiveFactory adjectiveFactory) : base(adjectiveFactory)
        {
            Blueprints = Compiler.Instance.Deserializer.Deserialize<List<RoomBlueprint>>(yaml);
        }
    }
}