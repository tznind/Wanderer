using System.Collections.Generic;
using System.Linq;
using Wanderer.Compilation;
using Wanderer.Systems;

namespace Wanderer.Behaviours
{
    public class BehaviourEventHandler
    {
        public List<ICondition<SystemArgs>> Condition {get;set;}

        public List<IEffect> Effect {get;set;}

        public void Fire(SystemArgs args)
        {
            if(Effect != null && (Condition == null || Condition.All(c=>c.IsMet(args.World,args))))
                foreach(var e in Effect)
                    e.Apply(args);
        }
    }
}