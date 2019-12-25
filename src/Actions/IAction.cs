using StarshipWanderer.Actors;
using StarshipWanderer.Behaviours;

namespace StarshipWanderer.Actions
{
    public interface IAction
    {
        string Name { get; set; }
        IActor PerformedBy { get; set; }

        CancellationStatus Cancelled { get; set; }
        
        void Push(IUserinterface ui, ActionStack stack);

        void Pop(IUserinterface ui,ActionStack stack);
    }
}