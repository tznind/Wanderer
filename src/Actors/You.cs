using StarshipWanderer.Places;

namespace StarshipWanderer.Actors
{
    public class You : Actor
    {
        public You():base( "Wanderer")
        {
            
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
