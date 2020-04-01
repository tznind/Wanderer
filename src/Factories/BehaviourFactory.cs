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
            base.HandleInheritance(blueprint);

            var instance  = new Behaviour(onto)
            {
                Name = blueprint.Name
            };
            
            AddBasicProperties(world,instance,blueprint,"evaluate");

            onto.BaseBehaviours.Add(instance);

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