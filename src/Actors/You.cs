using System;
using System.Text;
using StarshipWanderer.Actors;

namespace StarshipWanderer
{
    public class You : IActor
    {

        public string Name { get; set; } = "Wanderer";

        public int Loyalty { get; set; } = 50;
        public int Acting { get; set; } = 10;

        public int Fighting { get; set; } = 0;

        public int Corruption { get; set; } = 1;

    }
}
