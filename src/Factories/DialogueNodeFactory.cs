using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Dialogues;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;

namespace Wanderer.Factories
{
    public class DialogueNodeFactory
    {
        public DialogueNode Create(DialogueNodeBlueprint blueprint)
        {
            var node = new DialogueNode
            {
                Identifier = blueprint.Identifier
            };


            node.Body = blueprint.Body.Select(Create).ToList();

            node.Condition.AddRange(blueprint.Condition.SelectMany(c=>c.Create()));

            foreach(var optionBlueprint in blueprint.Options)
            {
                node.Options.Add(Create(optionBlueprint));
            }

            return node;

        }

        public TextBlock Create(TextBlockBlueprint blueprint)
        {
            var body = new TextBlock
            {
                Text = blueprint.Text
            };

            body.Condition.AddRange(blueprint.Condition.SelectMany(b=>b.Create()));
            return body;
        }

        public DialogueOption Create(DialogueOptionBlueprint blueprint)
        {
            var option = new DialogueOption
            {
                Attitude = blueprint.Attitude,
                Destination = blueprint.Destination,
                SingleUse = blueprint.SingleUse,
                Text = blueprint.Text,
                Transition = blueprint.Transition,
            };


            option.Condition.AddRange(blueprint.Condition.SelectMany(c=>c.Create()));

            option.Effect.AddRange(blueprint.Effect.SelectMany(c=>c.Create()));
            
            return option;
        }
    }
    
}