using StarshipWanderer.Items;

namespace StarshipWanderer.Adjectives
{
    public class Light : Adjective
    {
        public IItem Item { get; }

        public Light(IItem item) : base(item)
        {
            Item = item;

        }
    }
}