using StarshipWanderer.Actors;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Items
{
    /// <summary>
    /// Creates an item which represents 1 or more copies of something (e.g. credits).
    /// </summary>
    public class ItemStack:Item, IItemStack
    {
        /// <summary>
        /// The number copies of the item you have
        /// </summary>
        public int StackSize { get; set; }

        public ItemStack(string name,int stackSize) : base(name)
        {
            StackSize = stackSize;
        }

        /// <summary>
        /// Returns the BaseStats multiplied by the <see cref="StackSize"/>
        /// </summary>
        /// <param name="forActor"></param>
        /// <returns></returns>
        public override StatsCollection GetFinalStats(IActor forActor)
        {
            return base.GetFinalStats(forActor).Clone().SetAll(v => v * StackSize);
        }
    }
}