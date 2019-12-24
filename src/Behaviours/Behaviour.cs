using StarshipWanderer.Actions;

namespace StarshipWanderer.Behaviours
{
    public abstract class Behaviour<T> : IBehaviour where T : IAction
    {
        public abstract void OnPush(IUserinterface ui, ActionStack stack);
    }
}