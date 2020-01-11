using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actions
{
    public class ActorFactory : IActorFactory
    {
        public IItemFactory ItemFactory { get; set; }
        public AdjectiveFactory AdjectiveFactory { get; set; }

        public ActorFactory(IItemFactory itemFactory, AdjectiveFactory adjectiveFactory)
        {
            ItemFactory = itemFactory;
            AdjectiveFactory = adjectiveFactory;
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
            AddRandomAdjective(place, g);

            if(g.Has<Medic>(true))
                g.BaseStats[Stat.Savvy] = 20;
            
            place.World.Factions.AssignFactions(g);

            return g;
        }

        private IActor NewGuard(IPlace place, string name)
        {
            var g = new Npc(name,place);
            g.BaseStats[Stat.Loyalty] = 30;
            g.BaseStats[Stat.Fight] = 20;
            
            g.Items.Add(ItemFactory.Create(g));

            AddRandomAdjective(place, g);
            
            if(g.Has<Medic>(true))
                g.BaseStats[Stat.Savvy] = 20;

            //prevents anyone leaving the room unless loyalty is 10
            g.BaseBehaviours.Add(new ForbidBehaviour<Leave>(new FrameStatCondition(Stat.Loyalty,Comparison.LessThan, 10),g));

            place.World.Factions.AssignFactions(g);

            return g;
        }

        public void AddRandomAdjective(IPlace place, IActor actor)
        {
            //give the guard a random adjective
            var availableAdjectives = AdjectiveFactory.GetAvailableAdjectives(actor).ToArray();
            actor.Adjectives.Add(availableAdjectives[place.World.R.Next(availableAdjectives.Length)]);
        }
    }
}
