using System;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Systems;

namespace StarshipWanderer.Conditions
{
    public class RelationshipCondition : ICondition<SystemArgs>
    {
        public Comparison Condition { get; set; }
        public int Threshold { get; set; }

        public RelationshipCondition(Comparison condition, int threshold)
        {
            Condition = condition;
            Threshold = threshold;
        }

        public bool IsMet(SystemArgs dialogueArgs)
        {
            var talkingTo = dialogueArgs.Recipient as IActor;

            if (talkingTo == null)
                return false;

            var attitude = talkingTo.CurrentLocation.World.Relationships.SumBetween(talkingTo, dialogueArgs.AggressorIfAny);
            return Condition.IsMet(attitude, Threshold);
        }

        public string? SerializeAsConstructorCall()
        {
            return $"Relationship({Condition},{Threshold})";
        }
    }
}