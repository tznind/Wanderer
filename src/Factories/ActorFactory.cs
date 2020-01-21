using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Actors;
using StarshipWanderer.Adjectives;
using StarshipWanderer.Behaviours;
using StarshipWanderer.Conditions;
using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Factories
{
    public class ActorFactory : HasStatsFactory<IActor> ,IActorFactory
    {
        public IItemFactory ItemFactory { get; set; }

        public ActorFactory(IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
            ItemFactory = itemFactory;
        }
        public virtual void Create(IWorld world, IPlace place)
        {
            Guard(NewNpc(place));
            Guard(NewNpc(place));
            NewNpc(place);
            NewNpc(place);
        }
        
        private IActor NewNpc(IPlace place)
        {
            var world = place.World;

            var g = new Npc("Unnamed Npc",place);
            g.Name = place.ControllingFaction?.NameFactory?.GenerateName(world.R) ?? "Unnamed Npc";
            
            g.BaseStats[Stat.Loyalty] = 30;
            g.BaseStats[Stat.Fight] = 10;

            if (place.ControllingFaction != null)
                world.Factions.GetRandomFaction(world.R, place.ControllingFaction.Role);

            AddRandomAdjective(place, g);

            if(g.Has<Medic>(true))
                g.BaseStats[Stat.Savvy] = 20;
            
            //everyone in the location follows that faction
            if(place.ControllingFaction != null)
                g.FactionMembership.Add(place.ControllingFaction);
            
            return g;
        }

        private IActor Guard(IActor g)
        {
            g.BaseStats[Stat.Fight] += 10;
            g.Items.Add(ItemFactory.Create(g));
            
            //prevents anyone leaving the room unless loyalty is 10
            g.BaseBehaviours.Add(new ForbidBehaviour<Leave>(new FrameStatCondition(Stat.Loyalty,Comparison.LessThan, 10),g));
            
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
