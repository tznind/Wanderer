﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wanderer.Systems;

namespace Wanderer.Behaviours
{
    public class Behaviour : HasStats,IBehaviour
    {
        public IHasStats Owner { get; set; }

        public BehaviourEventHandler OnPushHandler {get;set;}

        public BehaviourEventHandler OnPopHandler {get;set;}

        public BehaviourEventHandler OnRoundEndingHandler {get;set;}

        /// <summary>
        /// Handy field for tracking behaviour progress e.g. getting hungry over
        /// time, three strikes and your out that kind of thing
        /// </summary>
        public int Count { get; set; }

        [JsonConstructor]
        protected Behaviour()
        {
            
        }
        public Behaviour(IHasStats owner)
        {
            Owner = owner;
        }

        public virtual void OnPush(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            OnPushHandler?.Fire(new ActionFrameSystemArgs(this,world,ui,stack,frame));
        }
        
        public virtual void OnPop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            OnPopHandler?.Fire(new ActionFrameSystemArgs(this,world,ui,stack,frame));
        }
        
        public virtual void OnRoundEnding(IWorld world,IUserinterface ui, Guid round)
        {
            OnRoundEndingHandler?.Fire(new BehaviourSystemArgs(this,world,ui,null,Owner,round));
        }
    }
}