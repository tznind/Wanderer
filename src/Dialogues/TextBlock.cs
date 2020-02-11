using System.Collections.Generic;
using Newtonsoft.Json;
using StarshipWanderer.Conditions;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Dialogues
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

        public ICondition[] Condition { get; set; } = new ICondition[0];

        public string Text { get; set; }
        
    }
}