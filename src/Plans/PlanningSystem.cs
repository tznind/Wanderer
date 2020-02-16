﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanderer.Actors;
using Wanderer.Systems;

namespace Wanderer.Plans
{

    /// <summary>
    /// System responsible for formulating a viable set of <see cref="Plan"/>s for
    /// <see cref="Npc"/>
    /// </summary>
    public class PlanningSystem : ISystem
    {
        /// <summary>
        /// Plans that can be picked from
        /// </summary>
        public List<Plan> Plans = new List<Plan>();

        public Guid Identifier { get; set; } = new Guid("3100f9db-a46b-4b6e-8e34-1222672a753c");
        
        public void Apply(SystemArgs args)
        {
            var actor = (Npc) args.Recipient;
            
            //clear any old plans
            actor.Plan = null;
            var viablePlans = new Dictionary<Plan,Frame>();

            foreach (var plan in Plans)
            {
                //if the plan is viable
                if (plan.Condition.TrueForAll(c => c.IsMet(args)))
                {
                    //then this is what we would do
                    var frame = plan.DoFrame.GetFrame(args);

                    //do some sanity checking, can we pick that action?
                    if (actor.GetFinalActions().Any(a => a.AreIdentical(frame.Action)))
                    {
                        //yes then it's viable
                        viablePlans.Add(plan,frame);
                    }
                }
            }

            //if we have a viable plan pick the best one
            if (viablePlans.Any())
                actor.Plan = 
                    viablePlans
                        .OrderByDescending(p => p.Key.Weight)
                        .First().Value;
        }
    }
}
