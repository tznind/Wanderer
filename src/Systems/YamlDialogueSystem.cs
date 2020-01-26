using System;
using StarshipWanderer.Conditions;
using StarshipWanderer.Dialogues;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Systems
{
    public class YamlDialogueSystem : DialogueSystem
    {
        public YamlDialogueSystem(params string[] dialogueYaml)
        {
            var de = new DeserializerBuilder()
                .WithTypeConverter(new ConditionYamlTypeConverter())
                .Build();

            if(dialogueYaml != null)
                foreach (string yaml in dialogueYaml)
                {
                    try
                    {
                        foreach (var dialogueNode in de.Deserialize<DialogueNode[]>(yaml)) 
                            AllDialogues.Add(dialogueNode);
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("Error in dialogue yaml:" + e.Message,e);
                    }
                }
        }

        public string Serialize()
        {
            var serializer = new SerializerBuilder()
                .WithTypeConverter(new ConditionYamlTypeConverter())
                .Build();

            return serializer.Serialize(this.AllDialogues);
        }
    }
}