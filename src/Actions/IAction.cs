using StarshipWanderer.Behaviours;

namespace StarshipWanderer.Actions
{
    public interface IAction
    {
        string Name { get; }

        void Push(IUserinterface ui, ActionStack stack);

        void Pop(IUserinterface ui,ActionStack stack);
    }
}