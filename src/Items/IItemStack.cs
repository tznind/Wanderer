namespace StarshipWanderer.Items
{
    public interface IItemStack : IItem
    {
        int StackSize { get; set; }
        bool CanCombine(IItemStack other);

        void Combine(IItemStack s2, IWorld world);
    }
}