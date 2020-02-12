using System;
using Wanderer.Actors;

namespace Wanderer.Systems
{
    class NegotiationSystem : INegotiationSystem
    {
        public void Apply(SystemArgs args)
        {
            var negotiation = (NegotiationSystemArgs) args;

            //how much do they like you? if they like you they are more likely to do what you say
            var actorRecipient = ((IActor)args.Recipient);
            var world = actorRecipient.CurrentLocation.World;

            var loveForYou = world.Relationships.SumBetween(actorRecipient,args.AggressorIfAny);
            

            //You need a high negotiation intensity to persuade people to perform extreme actions
            //Either extremely nasty or extremely nice
            var needed = Math.Abs(negotiation.Proposed.Attitude);

            needed -= loveForYou;

            if (negotiation.Intensity < needed)
            {
                negotiation.RejectNegotiation($"Insufficient persuasion (Needed {needed}, Had {negotiation.Intensity})");
            }
        }
    }
}