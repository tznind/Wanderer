namespace StarshipWanderer.Items
{
    public interface IItemStack : IItem
    {
        int StackSize { get; set; }
    }
}