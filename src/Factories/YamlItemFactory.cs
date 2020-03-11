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
        private YamlItemFactory(IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
            
        }

        public YamlItemFactory(string yaml, IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
            Blueprints = Compiler.Instance.Deserializer.Deserialize<ItemBlueprint[]>(yaml).ToList();
        }
    }
}