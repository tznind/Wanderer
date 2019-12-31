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
        public void Create(IWorld world, IPlace place)
        {
            NewGuard(place,"Guard 1");
            NewGuard(place,"Guard 2");
            NewWorker(place,"Worker 1");
            NewWorker(place,"Worker 2");
        }

        private IActor NewWorker(IPlace place, string name)
        {
            var g = new Npc(name,place);
            
            g.BaseStats[Stat.Loyalty] = 30;
            g.BaseStats[Stat.Fight] = 10;
            
            return g;
        }

        private IActor NewGuard(IPlace place, string name)
        {
            var g = new Npc(name,place);
            g.BaseStats[Stat.Loyalty] = 30;
            g.BaseStats[Stat.Fight] = 20;

            //prevents anyone leaving the room unless loyalty is 10
            g.AddBehaviour(new ForbidBehaviour<Leave>(new FrameStatCondition(Stat.Loyalty,Comparison.LessThan, 10),g));

            return g;
        }

    }
}
