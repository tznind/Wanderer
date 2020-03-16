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
        private YamlRoomFactory()
        {

        }

        public YamlRoomFactory(string yaml)
        {
            Blueprints = Compiler.Instance.Deserializer.Deserialize<List<RoomBlueprint>>(yaml);
        }
    }
}