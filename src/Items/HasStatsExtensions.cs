using StarshipWanderer.Actions;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Items
{
    public static class HasStatsExtensions
    {
        public static T With <T>(this T i, IAction action) where T:IHasStats
        {
            i.BaseActions.Add(action);
            return i;
        }

        public static T With<T>(this T i, Stat stat, double value) where T:IHasStats
        {
            i.BaseStats[stat] = value;
            return i;
        }

        public static T With <T>(this T i, IItemSlot slot) where T:IItem
        {
            i.Slot = slot;
            return i;
        }
    }
}