using System;
using System.Linq;
using StarshipWanderer.Actors;
using StarshipWanderer.Extensions;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Places;
using StarshipWanderer.Relationships;

namespace StarshipWanderer.Factories
{
    public class ActorFactory : HasStatsFactory<IActor> ,IActorFactory
    {
        public ActorBlueprint[] Blueprints { get; set; }
        
        public IItemFactory ItemFactory { get; set; }

        public ActorFactory(IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
            ItemFactory = itemFactory;
        }
        
        public virtual void Create(IWorld world, IPlace place,IFaction faction)
        {
            int numberOfNpc = Math.Max(1,world.R.Next(5));

            if(Blueprints.Any())
                for (int i = 0; i < numberOfNpc; i++)
                    Create(world, place, faction, Blueprints.GetRandom(world.R));
        }

        public IActor Create(IWorld world, IPlace place,IFaction faction, ActorBlueprint blueprint)
        {
            var npc = new Npc(blueprint.Name, place);

            AddBasicProperties(npc, blueprint, world,"talk");

            if (faction != null)
                npc.FactionMembership.Add(faction);
            
            if(string.IsNullOrWhiteSpace(npc.Name))
                npc.Name = faction?.NameFactory?.GenerateName(world.R) ?? "Unnamed Npc";

            foreach (var blue in blueprint.Items) 
                SpawnItem(world,npc, blue);

            //plus give them one more random thing that fits the faction
            if (ItemFactory.Blueprints.Any()) 
                SpawnItem(world,npc, ItemFactory.Blueprints.GetRandom(world.R));

            return npc;
        }

        private void SpawnItem(IWorld world,IActor actor, ItemBlueprint blue)
        {
            var item = ItemFactory.Create(world, blue);
            actor.Items.Add(item);

            if (actor.CanEquip(item, out _))
                item.IsEquipped = true;
        }
    }
}
