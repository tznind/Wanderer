using System;
using StarshipWanderer.Actors;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Dialogues.Tokens
{
    public class DescribeRelationshipToken : IDialogueToken
    {
        public string[] Tokens => new []{"DescribeRelationship"};
        public string GetReplacement(SystemArgs dialogueArgs)
        {            
            var a = dialogueArgs.Recipient as IActor;

            if (a == null)
                return string.Empty;

            var result = a.CurrentLocation.World.Relationships.SumBetween(a, dialogueArgs.AggressorIfAny);

            if (Math.Abs(result) < 0.01)
                return "indifferent";
            
            if (result < 0)
                return "hostile";
            
            return "friendly";
        }
    }
}