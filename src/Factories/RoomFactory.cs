using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StarshipWanderer.Actions;
using StarshipWanderer.Extensions;
using StarshipWanderer.Items;
using StarshipWanderer.Places;

namespace StarshipWanderer.Factories
{
    public class RoomFactory: HasStatsFactory<IPlace>, IRoomFactory
    {
        public RoomFactory(IAdjectiveFactory adjectiveFactory):base(adjectiveFactory)
        {
        }

        public IPlace Create(IWorld world)
        {
            var room = _buildersList[world.R.Next(_buildersList.Count)](world);

            //give the room a random adjective
            var availableAdjectives = AdjectiveFactory.GetAvailableAdjectives(room).ToArray();
            room.Adjectives.Add(availableAdjectives[world.R.Next(0, availableAdjectives.Length)]);

            if (world.Factions.Any())
            {
                var faction = room.ControllingFaction ?? world.Factions.GetRandomFaction(world.R);
                faction.ActorFactory?.Create(world, room,faction);

                var itemFactory = faction.ActorFactory?.ItemFactory;
                if(itemFactory!=null && itemFactory.Blueprints.Any())
                    room.Items.Add(itemFactory.Create(itemFactory.Blueprints.GetRandom(world.R)));
            }
            
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