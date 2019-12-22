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

        public RoomFactory RoomFactory { get; } = new RoomFactory();

        public Random R { get; } = new Random(100);

        public You Player { get;}

        public World(You player)
        {
            Player = player;
            CurrentLocation = RoomFactory.Create(this);
        }

    }
}
