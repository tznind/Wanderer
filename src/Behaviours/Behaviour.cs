using StarshipWanderer.Actions;
using StarshipWanderer.Actors;

namespace StarshipWanderer.Behaviours
{
    public abstract class Behaviour : IBehaviour
    {
        public IActor Owner { get; set; }

        protected Behaviour(IActor owner)
        {
            Owner = owner;
        }

        public abstract void OnPush(IUserinterface ui, ActionStack stack,Frame frame);
    }
}