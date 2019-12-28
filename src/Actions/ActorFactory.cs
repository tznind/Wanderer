using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actions
{
    public class ActorFactory : IActorFactory
    {
        public IEnumerable<IActor> Create(IWorld world, IPlace place)
        {
            yield return NewGuard(world,"Guard 1");
            yield return NewGuard(world,"Guard 2");
            yield return NewWorker(world,"Worker 1");
            yield return NewWorker(world,"Worker 2");
        }

        private IActor NewWorker(IWorld world, string name)
        {
            var g = new Actor(world,name);
            
            g.BaseStats[Stat.Loyalty] = 30;
            g.BaseStats[Stat.Fight] = 10;

            g.BaseActions.Add(new LoadGunsAction(world,g));

            return g;
        }

        private IActor NewGuard(IWorld world, string name)
        {
            var g = new Actor(world,name);
            g.BaseStats[Stat.Loyalty] = 30;
            g.BaseStats[Stat.Fight] = 20;

            //prevents anyone leaving the room unless loyalty is 10
            g.AddBehaviour(new ForbidBehaviour<Leave>(new ActionStatCondition(Stat.Loyalty,Comparison.LessThan, 10),g));

            return g;
        }

    }
}
