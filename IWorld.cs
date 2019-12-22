using System;
using StarshipWanderer.Places;

namespace StarshipWanderer
{
    public interface IWorld
    {
        Random R { get; }

        You Player { get; }
        IPlace CurrentLocation { get; set; }

        RoomFactory RoomFactory { get; } 
    }
}