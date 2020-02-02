﻿using System;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Systems
{
    public class NegotiationSystemArgs : SystemArgs
    {
        /// <summary>
        /// Output variable, true if the results of the negotiation indicate
        /// willingness to go ahead with whatever was proposed
        /// </summary>
        public bool Willing { get; private set; } = true;
        
        /// <summary>
        /// Output variable, should contain reason why <see cref="Willing"/>
        /// is false (e.g. insufficiently convincing)
        /// </summary>
        public string WillingReason { get; private set; }

        /// <summary>
        /// What is proposed?
        /// </summary>
        public Frame Proposed { get;set; }

        public NegotiationSystemArgs(IUserinterface ui, double intensity, IActor aggressorIfAny, IHasStats recipient, Guid round) : base(ui, intensity, aggressorIfAny, recipient, round)
        {
        }

        /// <summary>
        /// Sets <see cref="Willing"/> to false and records the <see cref="WillingReason"/>
        /// </summary>
        /// <param name="reason"></param>
        public void RejectNegotiation(string reason)
        {
            WillingReason = reason;
            Willing = false;
        }

    }
}