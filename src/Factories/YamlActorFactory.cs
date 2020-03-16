using System.Collections.Generic;
using Newtonsoft.Json;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;
using YamlDotNet.Serialization;

namespace Wanderer.Factories
{
    public class YamlActorFactory : ActorFactory
    {

        [JsonConstructor]
        private YamlActorFactory()
        {

        }

        public YamlActorFactory(string yaml, string yamlDefaultSlots)
        {
            
            Blueprints = Compiler.Instance.Deserializer.Deserialize<List<ActorBlueprint>>(yaml);

            if(!string.IsNullOrWhiteSpace(yamlDefaultSlots))
                DefaultSlots = Compiler.Instance.Deserializer.Deserialize<SlotCollection>(yamlDefaultSlots);
        }

    }
}