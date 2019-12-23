namespace StarshipWanderer.Actions
{
    public abstract class Action : IAction
    {
        public IWorld World { get; }
        public string Name { get; protected set; }

        protected Action(IWorld world)
        {
            World = world;
            Name = GetType().Name.Replace("Action", "");
        }
        public abstract void Perform(IUserinterface ui);
    }
}