using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using Newtonsoft.Json;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace StarshipWanderer
{
    public class World : IWorld
    {
        public Map Map { get; set;} = new Map();

        public IPlace CurrentLocation { get; set; }

        public IRoomFactory RoomFactory { get; set; }

        public Random R { get; set; } = new Random(100);

        public You Player { get; set; }

        public World()
        {
            
        }

        public World(You player, IRoomFactory roomFactory)
        {
            Player = player;
            RoomFactory = roomFactory;
            CurrentLocation = RoomFactory.Create(this);
            Map.Add(new Point3(0,0,0),CurrentLocation);
            CurrentLocation.Occupants.Add(player);
        }

        /// <summary>
        /// Returns settings suitable for loading/saving worlds
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var config = new JsonSerializerSettings();
            config.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;     
            config.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            config.TypeNameHandling = TypeNameHandling.All;
            return config;
        }
    }
}
