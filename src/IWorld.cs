using System;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;

namespace StarshipWanderer
{
    public interface IWorld
    {
        Random R { get; set; }

        You Player { get; set; }
        IPlace CurrentLocation { get; set; }

        IRoomFactory RoomFactory { get; set; } 
    }
}