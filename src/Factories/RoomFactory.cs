using System;
using System.Collections.Generic;
using System.Linq;
using StarshipWanderer.Extensions;
using StarshipWanderer.Factories.Blueprints;
using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public class RoomFactory: HasStatsFactory<IPlace>, IRoomFactory
    {
        public RoomBlueprint[] Blueprints { get; set; } = new RoomBlueprint[0];

        public ActorFactory GenericActorFactory { get; set; }
        public ItemFactory GenericItemFactory { get; set; }

        public RoomFactory(IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
            GenericItemFactory = new ItemFactory(adjectiveFactory);
            GenericActorFactory = new ActorFactory(GenericItemFactory,adjectiveFactory);
        }


        public IPlace Create(IWorld world)
        {
            return Create(world,Blueprints.Where(Spawnable).ToArray().GetRandom(world.R));
        }

        public IPlace Create(IWorld world, RoomBlueprint blueprint)
        {
            if (blueprint == null)
                return new Room("Empty Room",world,'e');
            
            //pick blueprint faction (or random one if it isn't themed to a specific faction)
            var faction = blueprint.Faction != null
                ? world.Factions.Single(f => f.Identifier == blueprint.Faction)
                : world.Factions.GetRandomFaction(world.R);

            var room = new Room(blueprint.Name, world, blueprint.Tile) {ControllingFaction = faction};

            AddBasicProperties(room,blueprint,world,"look");

            if (faction != null)
            {
                //create some random NPCs
                faction.ActorFactory?.Create(world, room, faction,blueprint);

                var itemFactory = faction.ActorFactory?.ItemFactory;

                if (itemFactory != null && itemFactory.Blueprints.Any())
                {
                    //create some random items
                    var items = world.R.Next(3);
                    for (int i = 0; i < items; i++) 
                        room.Items.Add(itemFactory.Create(world, 
                            //using global items suitable to the faction
                            itemFactory.Blueprints
                                //or the room
                                .Union(blueprint.OptionalItems).ToList()
                                .GetRandom(world.R)));
                }
            }

            foreach(var a in blueprint.MandatoryActors)
                GenericActorFactory.Create(world, room, room.ControllingFaction, a,blueprint);
            
            foreach(var i in blueprint.MandatoryItems)
                room.Items.Add(GenericItemFactory.Create(world,i));

            return room;
        }
    }
}