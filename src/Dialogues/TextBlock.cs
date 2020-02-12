using System.Collections.Generic;
using Newtonsoft.Json;
using Wanderer.Compilation;
using Wanderer.Systems;

namespace Wanderer.Dialogues
{
    public class TextBlock
    {
        [JsonConstructor]
        public TextBlock()
        {

        }
        public TextBlock(string text)
        {
            Text = text;
        }

        public ICondition<SystemArgs>[] Condition { get; set; } = new ICondition<SystemArgs>[0];

        public string Text { get; set; }
        
    }
}