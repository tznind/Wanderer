using System;
using System.Collections.Generic;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actions
{
    public class ActorFactory : IActorFactory
    {
        public IItemFactory ItemFactory { get; }

        public ActorFactory(IItemFactory itemFactory)
        {
            ItemFactory = itemFactory;
        }
        public void Create(IWorld world, IPlace place)
        {
            NewGuard(place,"Guard " + world.R.Next(999));
            NewGuard(place,"Guard " + world.R.Next(999));
            NewWorker(place,"Worker " + world.R.Next(999));
            NewWorker(place,"Worker " + world.R.Next(999));
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

            g.Items.Add(ItemFactory.Create(g));

            //prevents anyone leaving the room unless loyalty is 10
            g.BaseBehaviours.Add(new ForbidBehaviour<Leave>(new FrameStatCondition(Stat.Loyalty,Comparison.LessThan, 10),g));

            return g;
        }

    }
}
