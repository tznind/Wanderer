using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Newtonsoft.Json;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace StarshipWanderer
{
    public class World : IWorld
    {
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
            CurrentLocation.Occupants.Add(player);
        }

        public string SaveToString()
        {
            return JsonConvert.SerializeObject(this);
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
