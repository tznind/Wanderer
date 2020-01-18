namespace StarshipWanderer.Actors
{
    public interface INameFactory
    {
        string GenerateName(IActor suitableFor);
    }
}