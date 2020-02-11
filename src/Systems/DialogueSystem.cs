using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using StarshipWanderer.Actors;
using StarshipWanderer.Compilation;
using StarshipWanderer.Dialogues;
using StarshipWanderer.Effects;
using StarshipWanderer.Extensions;

namespace StarshipWanderer.Systems
{
    public class DialogueSystem : IDialogueSystem
    {
        public IList<DialogueNode> AllDialogues { get; set; } = new List<DialogueNode>();

        public void Apply(SystemArgs args)
        {
            if (args.AggressorIfAny == null)
                return;

            if (args.AggressorIfAny is You)
            {
                var d = GetDialogue(args.Recipient.Dialogue.Next);
                    
                //if there is no main dialogue set or its conditions are not yet met, fall back on banter
                if(d == null || !d.Require.All(c=>c.IsMet(args)))
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
            if(node.Options.Any())
            {
                if (args.UserInterface.GetChoice("Dialogue", FormatString(args,node.Body), out DialogueOption chosen, node.Options.ToArray()))
                    Run(args, chosen);
                else
                {
                    //if user hits Escape just pick the first option for them :)
                    Run(args,node.Options.First());
                }

            }
            else
                args.UserInterface.ShowMessage("Dialogue",FormatString(args,node.Body));
        }

        protected virtual string FormatString(SystemArgs args,TextBlock[] body)
        {
            StringBuilder sb = new StringBuilder();

            foreach (TextBlock block in body)
                if (block.Condition.All(c => c.IsMet(args)))
                {
                    sb.Append(block.Text);
                    sb.Append(' ');
                }

            return Regex.Replace(sb.ToString(), @"{\s*(\w+)\s*}", m=>ReplaceProperty(m,args)).Trim();
        }

        private string ReplaceProperty(Match match, SystemArgs args)
        {

            var prop = args.GetType().GetProperty(match.Groups[1].Value);

            if(prop == null)
                throw new ParseException($"Unknown Property '{match.Groups[1].Value}'");

            return prop.GetValue(args)?.ToString() ?? "Null";
        }

        private void Run(SystemArgs args, DialogueOption option)
        {
            if (option.Attitude.HasValue)
            {
                var w = args.AggressorIfAny.CurrentLocation.World;
                w.Relationships.Apply(new SystemArgs(args.UserInterface,option.Attitude.Value,args.AggressorIfAny,args.Recipient,args.Round));
            }

            //apply effects of the dialogue choice
            foreach (IEffect effect in option.Effect) 
                effect.Apply(args);

            var d = GetDialogue(option.Destination);

            if (d != null) 
                Run(args,d);
        }


        public bool CanTalk(IActor actor, IActor other)
        {
            return true;
        }

        public IEnumerable<IActor> GetAvailableTalkTargets(IActor actor)
        {
            return actor.GetCurrentLocationSiblings(false).Where(o => CanTalk(actor, o));
        }

        public DialogueNode GetBanter(SystemArgs args)
        {
            var valid = GetDialogues(args.Recipient.Dialogue.Banter)
                .Where(d => d.Require.All(c => c.IsMet(args)))
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