using System;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Extensions;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Relationships;
using System.Collections.Generic;

namespace Wanderer.Factories
{
    public class ActorFactory : HasStatsFactory<IActor> ,IActorFactory
    {
        public List<ActorBlueprint> Blueprints { get; set; } = new List<ActorBlueprint>();
        
        public IItemFactory ItemFactory { get; set; }

        public SlotCollection DefaultSlots { get; set; } = new SlotCollection();

        public ActorFactory(IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
            ItemFactory = itemFactory;
        }
        
        public virtual void Create(IWorld world, IRoom place, IFaction faction, RoomBlueprint roomBlueprintIfAny)
        {
            int numberOfNpc = Math.Max(1,world.R.Next(5));

            var pickFrom = Blueprints;

            if (roomBlueprintIfAny != null)
                pickFrom = pickFrom.Union(roomBlueprintIfAny.OptionalActors).ToList();

            if(pickFrom.Any())
                for (int i = 0; i < numberOfNpc; i++)
                    Create(world, place, faction, pickFrom.GetRandom(world.R),roomBlueprintIfAny);
        }

        public IActor Create(IWorld world, IRoom place, IFaction faction, ActorBlueprint blueprint, RoomBlueprint roomBlueprintIfAny)
        {
            var npc = new Npc(blueprint.Name, place);

            AddBasicProperties(npc, blueprint, world,"talk");

            if (faction != null)
                npc.FactionMembership.Add(faction);
            
            if(string.IsNullOrWhiteSpace(npc.Name))
                npc.Name = faction?.NameFactory?.GenerateName(world.R) ?? "Unnamed Npc";

            foreach (var blue in blueprint.MandatoryItems) 
                SpawnItem(world,npc, blue);

            //plus give them one more random thing that fits the faction / actor
            var pickFrom = ItemFactory.Blueprints.Union(blueprint.OptionalItems).ToArray();

            if (roomBlueprintIfAny != null)
                pickFrom = pickFrom.Union(roomBlueprintIfAny.OptionalItems).ToArray();
            
            if (pickFrom.Any()) 
                SpawnItem(world,npc, pickFrom.GetRandom(world.R));

            npc.AvailableSlots = (blueprint.Slots ?? DefaultSlots)?.Clone() ?? new SlotCollection();
            
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
