using System;
using System.Collections.Generic;
using StarshipWanderer.Actors;
using StarshipWanderer.Places;
using StarshipWanderer.UI;

namespace StarshipWanderer
{
    public interface IWorld
    {
        Random R { get; set; }

        You Player { get; }

        IRoomFactory RoomFactory { get; set; }

        Map Map { get; }

        HashSet<IActor> Population { get; }
        void RunNpcActions(ActionStack stack, IUserinterface ui);
    }
}