﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Extensions;
using Wanderer.Factories.Blueprints;
using Wanderer.Relationships;
using Wanderer.Rooms;

namespace Wanderer.Factories
{
    public class RoomFactory: HasStatsFactory<RoomBlueprint,IRoom>, IRoomFactory
    {
        public IRoom Create(IWorld world)
        {
            return Create(world,Blueprints.Where(Spawnable).ToArray().GetRandom(world.R));
        }
        public IRoom Create(IWorld world, Point3 location)
        {
            var exact = Blueprints.FirstOrDefault(b => Equals(location, b.FixedLocation));

            if (exact != null)
                return Create(world, exact);

            return Create(world);
        }


        public IRoom Create(IWorld world, RoomBlueprint blueprint)
        {
            if (blueprint == null)
                return new Room("Empty Room",world,'e');
            
            HandleInheritance(blueprint);
            
            IFaction faction = null;

            //if not unaligned
            if (!blueprint.Unaligned)
            {
                //pick blueprint faction (or random one if it isn't themed to a specific faction)
                if (blueprint.Faction != null)
                {
                    var match = world.Factions.SingleOrDefault(f => f.Identifier == blueprint.Faction);
                    faction = match ?? throw new Exception($"Could not find Faction {blueprint.Faction} listed by blueprint {blueprint}");
                }
                else
                    faction = world.Factions.GetRandomFaction(world.R);
            }
            
            var room = new Room(blueprint.Name, world, blueprint.Tile) {ControllingFaction = faction};

            //does the blueprint override the leave directions?
            if (blueprint.LeaveDirections != null && blueprint.LeaveDirections.Any())
            {
                room.LeaveDirections.Clear();
                room.LeaveDirections = new HashSet<Direction>(blueprint.LeaveDirections);
            }

            AddBasicProperties(world,room,blueprint,"look");
            world.AdjectiveFactory.AddAdjectives(world,room, blueprint);

            //get some actors for the room
            world.ActorFactory.Create(world, room, faction,blueprint);

            //get some items for the room
            world.ItemFactory.Create(world, room, faction,blueprint);

            foreach(var a in blueprint.MandatoryActors)
                world.ActorFactory.Create(world, room, room.ControllingFaction, a,blueprint);
            
            foreach(var i in blueprint.MandatoryItems)
                room.Items.Add(world.ItemFactory.Create(world,i));

            return room;
        }

    }
}