using StarshipWanderer.Actors;

namespace StarshipWanderer.Items
{
    public interface IItem : IHasStats
    {
        IActor OwnerIfAny { get; set; }
    }
}