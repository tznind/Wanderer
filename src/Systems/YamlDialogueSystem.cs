using System;
using System.IO;
using Wanderer.Actions;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using YamlDotNet.Serialization;

namespace Wanderer.Systems
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

        public YamlDialogueSystem(params FileInfo[] files)
        {
            if(files != null)
                foreach (var fi in files)
                {
                    try
                    {
                        foreach (var dialogueNode in Compiler.Instance.Deserializer.Deserialize<DialogueNode[]>(File.ReadAllText(fi.FullName))) 
                            AllDialogues.Add(dialogueNode);
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException($"Error in dialogue yaml:{ e.Message } in file '{fi.FullName}'",e);
                    }
                }
        }
    }
}