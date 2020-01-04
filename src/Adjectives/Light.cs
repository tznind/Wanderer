using System.Linq;
using StarshipWanderer.Adjectives.RoomOnly;
using StarshipWanderer.Items;
using StarshipWanderer.Stats;

namespace StarshipWanderer.Adjectives
{
    public class Light : Adjective
    {
        public IItem Item { get; }

        public Light(IItem item) : base(item)
        {
            Item = item;
            BaseStats[Stat.Fight] = 10;
        }

        public override bool IsActive()
        {
            //only active in dark locations
            return base.IsActive() && Item.OwnerIfAny != null &&
                   Item.OwnerIfAny.CurrentLocation.Adjectives.OfType<Dark>().Any();
        }
    }
}