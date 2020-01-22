﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public class RoomFactory: HasStatsFactory<IPlace>, IRoomFactory
    {
        public IActorFactory GenericActorFactory { get; set; }
        public IItemFactory ItemFactory { get; }

        public RoomFactory(IActorFactory genericActorFactory,IItemFactory itemFactory, IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
            GenericActorFactory = genericActorFactory;
            ItemFactory = itemFactory;
        }

        public IPlace Create(IWorld world)
        {
            var room = _buildersList[world.R.Next(_buildersList.Count)](world);

            //give the room a random adjective
            var availableAdjectives = AdjectiveFactory.GetAvailableAdjectives(room).ToArray();
            room.Adjectives.Add(availableAdjectives[world.R.Next(0, availableAdjectives.Length)]);

            if (room.ControllingFaction?.ActorFactory != null)
            {
                //if we can create faction specific actors
                room.ControllingFaction.ActorFactory.Create(world, room);
            }
            else
                //some friends in the room with you
                GenericActorFactory.Create(world, room);

            //some free items
            ItemFactory.Create(room);

            return room;
        }

        private IReadOnlyList<Func<IWorld, IPlace>> _buildersList = new ReadOnlyCollection<Func<IWorld, IPlace>>(
            new List<Func<IWorld, IPlace>>
            {
                w =>
                    new Room("Gun Bay " + w.R.Next(5000), w, 'g')
                        {
                            ControllingFaction = w.Factions.GetRandomFaction(w.R)
                        }
                        .With(new LoadGunsAction()),
                w =>
                    new Room("Stair" + w.R.Next(5000), w, 's')
                        {
                            ControllingFaction = w.Factions.GetRandomFaction(w.R)
                        }
                        .AllowUpDown(true)
            });
    }
}