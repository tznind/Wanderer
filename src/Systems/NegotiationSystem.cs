using System;

namespace StarshipWanderer.Systems
{
    class NegotiationSystem : INegotiationSystem
    {
        public void Apply(SystemArgs args)
        {
            var negotiation = (NegotiationSystemArgs) args;

            //You need a high negotiation intensity to persuade people to perform extreme actions
            //Either extremely nasty or extremely nice
            var needed = Math.Abs(negotiation.Proposed.Attitude);
            if (negotiation.Intensity < needed)
            {
                negotiation.RejectNegotiation($"Insufficient persuasion (Needed {needed}, Had {negotiation.Intensity})");
            }
        }
    }
}