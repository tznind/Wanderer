using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wanderer.Actors;
using Wanderer.Rooms;
using Wanderer.Systems;

namespace Wanderer.Behaviours
{
    public class Behaviour : HasStats,IBehaviour
    {
        public IHasStats Owner { get; set; }

        public BehaviourEventHandler OnPushHandler {get;set;}

        public BehaviourEventHandler OnPopHandler {get;set;}

        public BehaviourEventHandler OnRoundEndingHandler {get;set;}

        public BehaviourEventHandler OnEnterHandler {get;set;}


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
            OnRoundEndingHandler?.Fire(new EventSystemArgs(this,world,ui,null,Owner,round));
        }

        public void OnEnter(IWorld world, IUserinterface ui, Guid round, IActor actor, IRoom room)
        {
            OnEnterHandler?.Fire(new EventSystemArgs(this,world,ui,actor,room,round));
        }
    }
}