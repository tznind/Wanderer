using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Wanderer.Actors;
using Wanderer.Compilation;
using Wanderer.Dialogues;
using Wanderer.Extensions;
using Wanderer.Factories;

namespace Wanderer.Systems
{
    public class DialogueSystem : IDialogueSystem
    {
        public IList<DialogueNode> AllDialogues { get; set; } = new List<DialogueNode>();

        public Guid Identifier { get; set; } = new Guid("f5677833-c81a-4588-a2ad-54b0215aa926");

        public void Apply(SystemArgs args)
        {
            if (args.AggressorIfAny == null)
                return;

            if (args.AggressorIfAny is You)
            {
                var d = GetDialogue(args.Recipient.Dialogue.Next);
                    
                //if there is no main dialogue set or its conditions are not yet met, fall back on banter
                if(d == null || !d.Require.All(c=>c.IsMet(args.World,args)))
                    d =  GetBanter(args);
                
                if (d != null)
                    Run(args,d);
                else
                {
                    args.UserInterface.ShowMessage("Dialogue", $"{args.Recipient} had nothing interesting to say");
                }
                
            }

        }

        public DialogueNode GetDialogue(Guid? g)
        {
            return g.HasValue ? AllDialogues.SingleOrDefault(d => d.Identifier == g) : null;
        }
        
        public IEnumerable<DialogueNode> GetDialogues(Guid[] guids)
        {
            if (guids == null)
                yield break;

            foreach (var guid in guids)
            {
                var match = AllDialogues.Where(d => d.Identifier == guid).ToArray();

                if (match.Length == 1)
                    yield return match[0];
                else 
                    throw new Exception($"Could not find dialogue '{guid}'");
            }
        }

        public void Run(SystemArgs args, DialogueNode node)
        {
            var options = node.GetOptionsToShow();

            if(options.Any())
            {
                if (args.UserInterface.GetChoice("Dialogue", FormatString(args,node.Body), out DialogueOption chosen, options))
                    Run(args, chosen);
                else
                {
                    //if user hits Escape just pick the first option for them :)
                    Run(args,options.First());
                }

            }
            else
                args.UserInterface.ShowMessage("Dialogue",FormatString(args,node.Body));
        }

        protected virtual string FormatString(SystemArgs args,IEnumerable<TextBlock> body)
        {
            StringBuilder sb = new StringBuilder();

            foreach (TextBlock block in body)
                if (block.Condition.All(c => c.IsMet(args.World,args)))
                {
                    sb.Append(block.Text);
                    sb.Append(' ');
                }

            return Regex.Replace(sb.ToString(), @"{([^}]+)}", m=>ReplaceWithLua(m,args)).Trim();
        }

        private string ReplaceWithLua(Match match, SystemArgs args)
        {
            var code = match.Groups[1].Value.Trim();

            if(!code.StartsWith("return"))
                code = "return " + code;

            using(var lua = new LuaFactory().Create(args.World,args))
                return lua.DoString(code)?.ElementAt(0)?.ToString();
        }

        private void Run(SystemArgs args, DialogueOption option)
        {
            if (option.Attitude.HasValue)
            {
                args.World.Relationships.Apply(new SystemArgs(args.World,args.UserInterface,option.Attitude.Value,args.AggressorIfAny,args.Recipient,args.Round));
            }

            if (option.SingleUse)
                option.Exhausted = true;

            //apply effects of the dialogue choice
            foreach (IEffect effect in option.Effect) 
                effect.Apply(args);

            var d = GetDialogue(option.Destination);

            if (d != null) 
                Run(args,d);
        }


        public DialogueNode GetBanter(SystemArgs args)
        {
            var valid = GetDialogues(args.Recipient.Dialogue.Banter)
                .Where(d => d.Require.All(c => c.IsMet(args.World,args)))
                .ToArray();

            if (valid.Any())
            {
                var r= args.AggressorIfAny.CurrentLocation.World.R;
                return valid.GetRandom(r);
            }

            return null;
        }
    }
}