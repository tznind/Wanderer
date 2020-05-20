using System;
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
                Body = blueprint.Body,
                Identifier = blueprint.Identifier
            };

            node.Condition.AddRange(blueprint.Condition.SelectMany(c=>c.Create<SystemArgs>()));

            foreach(var optionBlueprint in blueprint.Options)
            {
                node.Options.Add(Create(optionBlueprint));
            }

            return node;

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


            option.Condition.AddRange(blueprint.Condition.SelectMany(c=>c.Create<SystemArgs>()));

            option.Effect.AddRange(blueprint.Effect.SelectMany(c=>c.Create()));
            
            return option;
        }
    }
    
}