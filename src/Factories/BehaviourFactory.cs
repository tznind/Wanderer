using System;
using System.Collections.Generic;
using Wanderer.Behaviours;
using Wanderer.Factories.Blueprints;

namespace Wanderer.Factories
{
    public class BehaviourFactory : HasStatsFactory<BehaviourBlueprint, IBehaviour>, IBehaviourFactory
    {
        public IBehaviour Create(IWorld world, IHasStats onto, BehaviourBlueprint blueprint)
        {
            var instance  = new Behaviour(onto);

            base.HandleInheritance(blueprint);
            
            AddBasicProperties(world,instance,blueprint,"evaluate");

            return instance;
        }

        public IBehaviour Create(IWorld world, IHasStats onto, string name)
        {
            return Create(world,onto,base.GetBlueprint(name));
        }

        public IBehaviour Create(IWorld world, IHasStats onto, Guid g)
        {
            return Create(world,onto,base.GetBlueprint(g));
        }
    }
}