using StarshipWanderer.Actions;

namespace StarshipWanderer.Items
{
    public static class HasStatsExtensions
    {
        public static T With <T>(this T i, IAction action) where T:IHasStats
        {
            i.BaseActions.Add(action);
            return i;
        }
    }
}