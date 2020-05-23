using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public class RevealEffect : IEffect
    {
        public Point3 Location { get; }

        public RevealEffect(Point3 location)
        {
            Location = location;
        }


        public void Apply(SystemArgs args)
        {
            args.World.Reveal(Location);
        }
    }
}