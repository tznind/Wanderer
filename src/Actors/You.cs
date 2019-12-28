using StarshipWanderer.Places;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Actors
{
    public class You : Actor
    {
        public You(IWorld world):base( world,"Wanderer")
        {
            BaseStats[Stat.Loyalty] = 10;
            BaseStats[Stat.Fight] = 10;
            BaseStats[Stat.Coerce] = 10;
        }

        public override bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options, int attitude)
        {
            //ask user through UI
            return ui.GetChoice(title,body,out chosen,options);
        }

        public override void Move(IWorld world, IPlace currentLocation, IPlace newLocation)
        {
            world.CurrentLocation = newLocation;
            base.Move(world, currentLocation, newLocation);
        }
    }
}
