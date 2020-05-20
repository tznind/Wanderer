using System.Collections.Generic;
using System.Linq;
using Wanderer.Dialogues;
using Wanderer.Systems;

namespace Wanderer.Factories.Blueprints
{
    public class TextBlockBlueprint
    {

        public List<ConditionBlueprint> Condition { get; set; } = new List<ConditionBlueprint>();

        public string Text { get; set; }

        public TextBlock Create()
        {
            var tb = new TextBlock(){
                Text = Text
            };

            tb.Condition.AddRange(Condition.SelectMany(b=>b.Create<SystemArgs>()));

            return tb;

        }
    }
}