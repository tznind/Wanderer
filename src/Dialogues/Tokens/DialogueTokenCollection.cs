using System;
using System.Collections.Generic;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Dialogues.Tokens
{
    public class DialogueTokenCollection : List<IDialogueToken>
    {
        public void Add(Func<SystemArgs, string> replacement, params string[] tokens)
        {
            Add(new SimpleDialogueToken(replacement, tokens));
        }
    }
}