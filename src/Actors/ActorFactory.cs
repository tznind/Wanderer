using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;
using StarshipWanderer.Items;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    public class ActorFactory : IActorFactory
    {
        public IItemFactory ItemFactory { get; set; }
        public IAdjectiveFactory AdjectiveFactory { get; set; }
        public INameFactory NameFactory { get; }

        public ActorFactory(IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory, INameFactory nameFactory)
        {
            ItemFactory = itemFactory;
            AdjectiveFactory = adjectiveFactory;
            NameFactory = nameFactory;
        }
        public void Create(IWorld world, IPlace place)
        {
            NewGuard(place,"Guard");
            NewGuard(place,"Guard");
            NewWorker(place,"Worker");
            NewWorker(place,"Worker");
        }

        private IActor NewWorker(IPlace place, string role)
        {
            var g = new Npc(role,place);
            g.Name = NameFactory.GenerateName(g) + $"({role})";
            
            g.BaseStats[Stat.Loyalty] = 30;
            g.BaseStats[Stat.Fight] = 10;
            AddRandomAdjective(place, g);

            if(g.Has<Medic>(true))
                g.BaseStats[Stat.Savvy] = 20;
            
            place.World.Factions.AssignFactions(g);

            return g;
        }

        private IActor NewGuard(IPlace place,string role)
        {
            var g = new Npc(role,place);
            
            g.Name = NameFactory.GenerateName(g) + $"({role})";

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
