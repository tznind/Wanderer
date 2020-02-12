using System;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Stats;

namespace Wanderer.Items
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

        public bool CanCombine(IItemStack other)
        {
            //don't combine with yourself!
            if (other == this)
                return false;

            //don't combine erased stuff!
            if (this.IsErased || other.IsErased)
                return false;

            return other.AreIdentical(this);
        }

        public void Combine(IItemStack s2, IWorld world)
        {
            if(!CanCombine(s2))
                throw new ArgumentException("Unable to combine objects because they were were not compatible");

            StackSize += s2.StackSize;
            world.Erase(s2);
        }

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

        public override string ToString()
        {
            return base.ToString() + "(" + StackSize +")";
        }
    }
}