namespace StarshipWanderer.Actions
{
    public interface IAction
    {
        string Name { get; }

        void Perform(IUserinterface ui);
    }
}