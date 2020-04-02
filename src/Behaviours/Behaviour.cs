using System;
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
            if(OnPushHandler != null)
                OnPushHandler.Fire(new ActionFrameSystemArgs(world,ui,stack,frame));
        }


        public virtual void OnPop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            if(OnPopHandler != null)
                OnPopHandler.Fire(new ActionFrameSystemArgs(world,ui,stack,frame));

        }


        public virtual void OnRoundEnding(IWorld world,IUserinterface ui, Guid round)
        {
            if(OnRoundEndingHandler != null)
                OnRoundEndingHandler.Fire(new SystemArgs(world,ui,0,null,Owner,round));
        }
    }
}