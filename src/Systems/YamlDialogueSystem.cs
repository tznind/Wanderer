using System;
using StarshipWanderer.Actions;
using StarshipWanderer.Compilation;
using StarshipWanderer.Conditions;
using StarshipWanderer.Dialogues;
using YamlDotNet.Serialization;

namespace StarshipWanderer.Systems
{
    public class YamlDialogueSystem : DialogueSystem
    {
        public YamlDialogueSystem(params string[] dialogueYaml)
        {
            if(dialogueYaml != null)
                foreach (string yaml in dialogueYaml)
                {
                    try
                    {
                        foreach (var dialogueNode in Compiler.Instance.Deserializer.Deserialize<DialogueNode[]>(yaml)) 
                            AllDialogues.Add(dialogueNode);
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("Error in dialogue yaml:" + e.Message,e);
                    }
                }
        }
    }
}