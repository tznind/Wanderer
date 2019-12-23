namespace StarshipWanderer.Places
{
    public interface IRoomFactory
    {
        IPlace Create(IWorld world);
    }
}