using System;
using Newtonsoft.Json;

namespace Wanderer.Behaviours
{
    public class Behaviour : HasStats,IBehaviour
    {
        public IHasStats Owner { get; set; }
        
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

        }


        public virtual void OnPop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {

        }


        public virtual void OnRoundEnding(IWorld world,IUserinterface ui, Guid round)
        {
        }
    }
}