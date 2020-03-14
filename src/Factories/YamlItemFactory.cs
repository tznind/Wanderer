using System.Linq;
using Newtonsoft.Json;
using Wanderer.Actions;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Factories.Blueprints;
using YamlDotNet.Serialization;

namespace Wanderer.Factories
{
    public class YamlItemFactory : ItemFactory
    {
        [JsonConstructor]
        private YamlItemFactory()
        {
            
        }

        public YamlItemFactory(string yaml)
        {
            Blueprints = Compiler.Instance.Deserializer.Deserialize<ItemBlueprint[]>(yaml).ToList();
        }
    }
}