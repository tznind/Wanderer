using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using StarshipWanderer.Places;

namespace StarshipWanderer
{
    public class World : IWorld
    {
        public IPlace CurrentLocation { get; set; }

        public IRoomFactory RoomFactory { get; set; }

        public Random R { get; } = new Random(100);

        public You Player { get; set; }

        public World()
        {
            
        }

        public World(You player, IRoomFactory roomFactory)
        {
            Player = player;
            RoomFactory = roomFactory;
            CurrentLocation = RoomFactory.Create(this);
            CurrentLocation.Occupants.Add(player);
        }
    }
}
