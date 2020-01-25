using System;
using System.Linq;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Dialogues.Tokens
{
    /// <summary>
    /// Describes a keyword that can appear in dialogues that should
    /// be substituted e.g."this" => Name of person/item you are talking
    /// to
    /// </summary>
    public class SimpleDialogueToken : IDialogueToken
    {

        /// <summary>
        /// The keyword (or sequence) that will be replaced e.g."this"
        /// </summary>
        public string[] Tokens { get; set; }


        /// <summary>
        /// The substitution function when dialogue occurs with the
        /// <see cref="SystemArgs.Recipient"/>.  <see cref="SystemArgs.AggressorIfAny"/>
        /// is the actor that initiated the dialogue
        /// </summary>
        public Func<SystemArgs,string> Replacement { get; set; }

        
        public SimpleDialogueToken(Func<SystemArgs, string> replacement, params string[] tokens)
        {
            if(!tokens.Any())
                throw new ArgumentException("There must be at least one token");
            Replacement = replacement;
            Tokens = tokens;
        }

        
        public string GetReplacement(SystemArgs dialogueArgs)
        {
            return Replacement(dialogueArgs);
        }
    }
}