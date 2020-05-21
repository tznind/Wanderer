using System.Collections.Generic;
using System.Linq;
using Wanderer.Dialogues;
using Wanderer.Systems;

namespace Wanderer.Factories.Blueprints
{
    /// <summary>
    /// Describes how to create instances of <see cref="TextBlock"/>
    /// </summary>
    public class TextBlockBlueprint
    {
        /// <summary>
        /// List of conditions that must be true before this sentence or paragraph should be included in the displayed text of a <see cref="DialogueNode"/>.  If no conditions then it is automatically included
        /// </summary>
        public List<ConditionBlueprint> Condition { get; set; } = new List<ConditionBlueprint>();

        /// <summary>
        /// The dialogue text to display to the user
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// Creates an instance of <see cref="TextBlock"/> from the blueprint settings
        /// </summary>
        /// <returns></returns>
        public TextBlock Create()
        {
            var tb = new TextBlock(){
                Text = Text
            };

            tb.Condition.AddRange(Condition.SelectMany(b=>b.Create()));

            return tb;

        }
    }
}